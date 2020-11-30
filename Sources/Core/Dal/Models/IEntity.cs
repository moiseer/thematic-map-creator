using System;

namespace Core.Dal.Models
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}
