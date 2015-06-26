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

      public static string EnsureCharacter( this string value, int position, char character, bool insert = false )
      {
         if ( ( position < 0 ) || ( position >= value.Length ) )
         {
            throw new ArgumentOutOfRangeException( "position" );
         }

         if ( value[position] != character )
         {
            var newValue = new StringBuilder( value.Substring( 0, position + ( position == value.Length - 1 ? 1 : 0 ) ) );
            newValue.Append( character );
            newValue.Append( value.Substring( position + ( position == value.Length - 1 || !insert ? 1 : 0 ) ) );
            return newValue.ToString();
         }

         return value;
      }

      public static string EnsureLastCharacter( this string value, char character, bool insert = false )
      {
         return value.EnsureCharacter( value.Length - 1, character, insert );
      }

      public static string EnsureFirstCharacter( this string value, char character, bool insert = false )
      {
         return value.EnsureCharacter( 0, character, insert );
      }
   }
}
