using ApiActividades.Models.Entities;

namespace ApiActividades.Repositories
{
    public class Repository<T> where T : class
    {
        public ItesrcneActividadesContext context { get; }

        public Repository(ItesrcneActividadesContext context)
        {
            this.context = context;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return context.Set<T>();
        }
        public virtual T? Get(object id)
        {
            return context.Find<T>(id);
        }
        public virtual void Insert(T entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            context.Update(entity);
            context.SaveChanges();
        }
        public virtual void Delete(T entity)
        {
            context.Remove(entity);
            context.SaveChanges();
        }

        public virtual void Delete(object id)
        {

            var entity = Get(id);
            if (entity != null)
            {
                Delete(entity);
            }

        }
    }
}
