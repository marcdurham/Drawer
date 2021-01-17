using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DumbCad.Entities
{
    public class EntityCollection : IEnumerable<Entity>
    {
        List<Entity> entities = new List<Entity>();

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entities.GetEnumerator();
        }
    }
}
