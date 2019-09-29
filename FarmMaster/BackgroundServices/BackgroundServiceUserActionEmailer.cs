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

namespace FarmMaster.BackgroundServices
{
    public class BackgroundServiceUserActionEmailer : IFarmBackgroundService
    {
        FarmMasterContext _context;
        IServiceSmtpClient _mail;

        public IFarmBackgroundServiceConfig Config => new IFarmBackgroundServiceConfig
        {
            DelayPerTicks = TimeSpan.FromMinutes(30),
            RestartOnException = true
        };

        public BackgroundServiceUserActionEmailer(
            FarmMasterContext context,
            IServiceSmtpClient mail
        )
        {
            this._context = context;
            this._mail = mail;
        }

        public async Task OnTickAsync(CancellationToken stoppingToken)
        {
            // Check if there are any new actions that the contact needs to be told of.
            while (this._context.ActionsAgainstContactInfo
                                .AsNoTracking()
                                .Any(a => !a.HasContactBeenInformed)
            )
            {
                // Get the first uninformed action, then get the rest of the uninformed actions for that particular contact.
                var first = this._context.ActionsAgainstContactInfo.First(a => !a.HasContactBeenInformed);
                var allActions = this._context.ActionsAgainstContactInfo
                                              .Where(a => a.ContactAffectedId == first.ContactAffectedId)
                                              .Where(a => !a.HasContactBeenInformed)
                                              .Include(a => a.ContactAffected)
                                               .ThenInclude(c => c.EmailAddresses)
                                              .Include(a => a.UserResponsible)
                                               .ThenInclude(u => u.Contact);

                // Send the email.
                await this._mail.SendToWithTemplateAsync(
                    allActions.First().ContactAffected.EmailAddresses.Select(e => e.Address),
                    FarmConstants.EmailTemplateNames.ContactInfoAudit,
                    "The latest audit log about your contact information.",
                    new EmailContactInfoAuditViewModel
                    {
                        ActionsTaken = allActions
                                       .Select(a => new EmailContactInfoAuditViewModel.Data
                                       {
                                           AdditionalInfo = a.AdditionalInfo,
                                           What = Convert.ToString(a.ActionType),
                                           When = a.DateTimeUtc.ToString("dd/MM/yyyy HH:mm:ss"),
                                           Who = a.UserResponsible.Contact.FirstNameWithAbbreviatedLastName,
                                           Why = a.Reason
                                       })
                    }
                );

                // Then register them as being informed.
                foreach (var action in allActions)
                    action.HasContactBeenInformed = true;
                this._context.SaveChanges();
            }
        }
    }
}
