using System;
using System.Text;
using System.Reflection;
using System.Collections;

namespace MobileAccess
{
   public static class Extensions
   {
      public static string ToStringFlags( this Enum value )
      {
         var hasFlags = false;
         var type = value.GetType();
         var attributes = type.GetCustomAttributes<System.Attribute>();
         foreach ( var attribute in attributes )
         {
            if ( attribute.GetType() == typeof( System.FlagsAttribute ) )
            {
               hasFlags = true;
               break;
            }
         }

         if ( !hasFlags )
         {
            return value.ToString();
         }

         var output = new StringBuilder();

         var members = type.GetMembers();

         for( var i = 0; i < members.Length; i++ )
         {
            if ( members[i].MemberType == MemberTypes.Field )
            {
               var fieldInfo = (FieldInfo) members[i];
               if ( !fieldInfo.IsSpecialName && fieldInfo.IsLiteral )
               {
                  var literal = (uint) fieldInfo.GetRawConstantValue();
                  if ( value.HasFlag( (Enum) Enum.ToObject( value.GetType(), literal ) ) )
                  {
                     output.AppendFormat( "{0}|", fieldInfo.Name );
                  }
               }
            }
         }

         output.Remove( output.Length - 1, 1 );

         return output.ToString();
      }

      public static string DelimitedString( this IEnumerable enumerable, string delimiter = ", " )
      {
         var output = new StringBuilder();

         foreach ( var item in enumerable )
         {
            if ( item == null )
            {
               output.AppendFormat( "{0}{1}", "<[null]>", delimiter );
            }
            else
            {
               output.AppendFormat( "{0}{1}", item.ToString(), delimiter );
            }
         }

         output.Remove( output.Length - delimiter.Length, delimiter.Length );

         return output.ToString();
      }
   }
}
