using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZ204_Demo_API.Models
{
  public class User
  {
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Post> Posts { get; set; }

    public static void CopyIfDifferent(User target, User source)
    {
      foreach (var prop in source.GetType().GetProperties().Where(p => p.GetValue(source) != null ))
      {
        var targetValue = GetPropValue(target, prop.Name);
        var sourceValue = GetPropValue(source, prop.Name);
        if (!targetValue.Equals(sourceValue) && !prop.PropertyType.IsArray)
        {
          SetPropertyValue(target, prop.Name, sourceValue);
        }
      }
    }
    public static object GetPropValue(object src, string propName)
    {
      return src.GetType().GetProperty(propName).GetValue(src, null);
    }
    public static void SetPropertyValue(object obj, string propName, object value)
    {
      obj.GetType().GetProperty(propName).SetValue(obj, value, null);
    }
  }  
}
