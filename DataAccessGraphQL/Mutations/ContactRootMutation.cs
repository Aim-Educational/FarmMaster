﻿using DataAccess;
using DataAccess.Constants;
using DataAccessGraphQL.Util;
using DataAccessLogic;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessGraphQL.Mutations
{
    public class ContactRootMutation : ObjectGraphType<Contact>
    {
        readonly DataAccessUserContext _context;
        readonly IContactManager       _contacts;
        readonly INoteManager          _notes;
        readonly IUnitOfWork           _unitOfWork;

        public ContactRootMutation(
            GraphQLUserContextAccessor   accessor,
            IContactManager              contacts,
            INoteManager                 notes,
            IUnitOfWork                  unitOfWork
        )
        {
            this._context       = accessor.Context;
            this._contacts      = contacts;
            this._notes         = notes;
            this._unitOfWork    = unitOfWork;

            this.AddNoteMutations();
        }

        private void AddNoteMutations()
        {
            this.AddNoteMutationsAsync(
                this._context,
                this._unitOfWork,
                this._notes,
                ctx => {
                    ctx.Source.NoteOwner = ctx.Source.NoteOwner ?? new NoteOwner();
                    this._contacts.Update(ctx.Source);
                    
                    return ctx.Source.NoteOwner;
                }
            );
        }
    }
}