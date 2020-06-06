using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoService.Repository
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetItemList(); // получение всех объектов
        T GetItem(int id); // получение одного объекта по id
        void Create(T item); 
        void Update(T item);
        void Delete(int id); // удаление объекта по id

    }
}
