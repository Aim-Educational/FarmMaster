using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLogic
{
    public interface INoteManager : ICrudAsync<NoteEntry>
    {
    }

    public class NoteManager : DbContextCrud<NoteEntry, FarmMasterContext>, INoteManager
    {
        readonly IUnitOfWork _unitOfWork;

        public NoteManager(FarmMasterContext db, IUnitOfWork unitOfWork) : base(db)
        {
            this._unitOfWork = unitOfWork;
        }
    }
}
