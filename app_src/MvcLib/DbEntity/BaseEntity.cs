using System;
using System.Collections.Generic;
using System.Text;

namespace MvcLib.DbEntity
{

    /// <summary>
    /// basic entity
    /// 
    /// </summary>
    /// <typeparam name="T">Vm</typeparam>
    abstract public class BaseEntity<T>
    {
       public abstract T Vm { get; }
     }
}
