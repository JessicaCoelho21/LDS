using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebDAO.Interfaces
{
    public interface IDAO<T> : IDisposable where T : class, new()
    {
        //CRUD
        T Create(T model);
        T FindById(params Object[] keys);
        Collection<T> GetAll();
        void Update(T model);
        bool Delete(T model);
    }
}
