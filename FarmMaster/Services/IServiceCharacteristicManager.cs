using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Services
{
    public interface IServiceCharacteristicManager : IServiceEntityManager<AnimalCharacteristic>, IServiceEntityManagerFullDeletion<AnimalCharacteristic>
    {
        AnimalCharacteristic CreateFromHtmlString(AnimalCharacteristicList list, string name, DynamicField.Type type, string htmlString);
        void FullDeleteById(AnimalCharacteristicList list, int id);
    }

    public class ServiceCharacteristicManager : IServiceCharacteristicManager
    {
        readonly FarmMasterContext _context;

        public ServiceCharacteristicManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public AnimalCharacteristic CreateFromHtmlString(AnimalCharacteristicList list, string name, DynamicField.Type type, string htmlString)
        {
            if(list.Characteristics.Any(c => c.Name == name))
                throw new Exception("A characteristic with that name already exists.");

            var factory = new DynamicFieldFactory();
            var data = factory.FromTypeAndHtmlString(type, htmlString);
                   
            var chara = new AnimalCharacteristic
            {
                Data = data,
                List = list,
                Name = name
            };

            this._context.Add(chara);
            this._context.SaveChanges();

            return chara;
        }

        public void FullDelete(AnimalCharacteristic entity)
        {
            this._context.Remove(entity);
            this._context.SaveChanges();
        }

        public void FullDeleteById(AnimalCharacteristicList list, int id)
        {
            var chara = list.Characteristics.FirstOrDefault(c => c.AnimalCharacteristicId == id);
            if(chara == null)
                throw new KeyNotFoundException($"Characteristic list does not contain characteristic with ID #{id}");

            this.FullDelete(chara);
        }

        public int GetIdFor(AnimalCharacteristic entity)
        {
            return entity.AnimalCharacteristicId;
        }

        public IQueryable<AnimalCharacteristic> Query()
        {
            // TBH, no real reason the query stuff needs to even be used with this entity.
            throw new NotImplementedException();
        }

        public IQueryable<AnimalCharacteristic> QueryAllIncluded()
        {
            // See above.
            throw new NotImplementedException();
        }

        public void Update(AnimalCharacteristic entity)
        {
            if(entity == null)
                throw new ArgumentNullException("entity");

            // Because of the slightly dodgy way we're handling the 'Data' field, it's safer to just always
            // assume it's been modified, instead of letting EF determine.
            this._context.Update(entity);
            this._context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
        }
    }
}
