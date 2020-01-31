using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Misc;
using FarmMaster.Services;
using FarmMaster.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FarmMaster.BackgroundServices
{
    public class BackgroundServiceUserActionEmailer : IFarmBackgroundService
    {
        FarmMasterContext _context;
        IServiceSmtpClient _mail;
        IOptions<IServiceSmtpDomainConfig> _domains;
        IServiceContactManager _contacts;

        public IFarmBackgroundServiceConfig Config => new IFarmBackgroundServiceConfig
        {
            DelayPerTicks = TimeSpan.FromMinutes(30),
            RestartOnException = true
        };

        public BackgroundServiceUserActionEmailer(
            FarmMasterContext context,
            IServiceSmtpClient mail,
            IOptions<IServiceSmtpDomainConfig> domains,
            IServiceContactManager contacts
        )
        {
            this._context  = context;
            this._mail     = mail;
            this._domains  = domains;
            this._contacts = contacts;
        }

        public async Task OnTickAsync(CancellationToken stoppingToken)
        {
            // Go over every unique email across all contacts, and consolidate all un-informed
            // actions. Then email the actions before marking them as informed.
            var actionsPerEmail = new Dictionary<string, IEnumerable<ActionAgainstContactInfo>>();
            foreach(var email in this._context.Emails
                                              .Select(e => e.Address)
                                              .Distinct()
            )
            {
                actionsPerEmail[email] = this._context.ActionsAgainstContactInfo
                                                      .Where(a => !a.HasContactBeenInformed)
                                                      .Where(a => a.ContactAffected.EmailAddresses.Any(e => e.Address == email))
                                                      .Where(a => !a.ContactAffected.UnsubscribedAddresses.Any(e => e.Address == email))
                                                      .Include(a => a.UserResponsible)
                                                       .ThenInclude(u => u.Contact)
                                                      .Include(a => a.ContactAffected)
                                                       .ThenInclude(c => c.EmailAddresses)
                                                      .Include(a => a.ContactAffected)
                                                       .ThenInclude(c => c.UnsubscribedAddresses)
                                                      .ToList(); // Have to do this as we're editing data that'd affect the query later on.
            }

            // For each email, also create an unsubscribe token to pass to the user.
            foreach(var kvp in actionsPerEmail.Where(kvp => kvp.Value.Any()))
            {
                var expires = DateTimeOffset.UtcNow.AddDays(7);
                var token   = this._contacts.GenerateUnsubscribeToken(kvp.Key, expires);

                await this._mail.SendToWithTemplateAsync(
                    new[] { kvp.Key },
                    FarmConstants.EmailTemplateNames.ContactInfoAudit,
                    "The latest audit log about your contact information.",
                    new EmailContactInfoAuditViewModel
                    {
                        ActionsTaken = kvp
                                       .Value
                                       .Select(a => new EmailContactInfoAuditViewModel.Data
                                       {
                                           AdditionalInfo = a.AdditionalInfo,
                                           What           = Convert.ToString(a.ActionType),
                                           When           = a.DateTimeUtc.ToString("dd/MM/yyyy HH:mm:ss"),
                                           Who            = a.UserResponsible.Contact.FirstNameWithAbbreviatedLastName,
                                           Why            = a.Reason
                                       }),
                        UnsubscribeUrl = $"{this._domains.Value.ContactUnsubscribe}{token.Token}"
                    }
                );

                foreach (var action in kvp.Value)
                {
                    action.HasContactBeenInformed = true;
                    this._context.Update(action);
                }
            }
            this._context.SaveChanges();
        }

        public Task OnShutdown()
        {
            return Task.CompletedTask;
        }
    }
}
