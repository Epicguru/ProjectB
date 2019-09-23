
using System;
using System.Reflection;

namespace ProjectB.Commands
{
    public class FInfo
    {
        public bool IsProperty { get { return prop != null; } }
        public bool IsStatic
        {
            get
            {
                if (IsProperty)
                {
                    var get = prop.GetGetMethod();
                    if (get != null)
                    {
                        return get.IsStatic;
                    }
                    else
                    {
                        var set = prop.GetSetMethod();
                        return set.IsStatic;
                    }
                }
                else
                {
                    return field.IsStatic;
                }
            }
        }
        public bool CanWrite
        {
            get
            {
                return !IsProperty || prop.CanWrite;
            }
        }
        public bool CanRead
        {
            get
            {
                return !IsProperty || prop.CanRead;
            }
        }
        public Type MemberType
        {
            get
            {
                return IsProperty ? prop.PropertyType : field.FieldType;
            }
        }

        private PropertyInfo prop;
        private FieldInfo field;

        public FInfo(PropertyInfo info)
        {
            this.prop = info;
        }

        public FInfo(FieldInfo info)
        {
            this.field = info;
        }

        public void SetValue(object instance, object value)
        {
            if (IsProperty)
                prop.SetValue(instance, value);
            else
                field.SetValue(instance, value);
        }

        public object GetValue(object instance)
        {
            if (IsProperty)
                return prop.GetValue(instance);
            else
                return field.GetValue(instance);
        }
    }
}
