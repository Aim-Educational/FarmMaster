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
    public class SpeciesRootMutation : ObjectGraphType<Species>
    {
        readonly DataAccessUserContext _context;
        readonly ISpeciesManager       _species;
        readonly INoteManager          _notes;
        readonly IUnitOfWork           _unitOfWork;

        public SpeciesRootMutation(
            GraphQLUserContextAccessor   accessor,
            ISpeciesManager              species,
            INoteManager                 notes,
            IUnitOfWork                  unitOfWork
        )
        {
            this._context       = accessor.Context;
            this._species       = species;
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
                    this._species.Update(ctx.Source);
                    
                    return ctx.Source.NoteOwner;
                }
            );
        }
    }
}