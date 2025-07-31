using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace IdentTestTask3
{
    internal class Program
    {
        private static IEnumerable<Type> GetStandardControls(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => typeof(Control).IsAssignableFrom(type) &&  // получаем все классы из сборки, которые наследуются от Control
                               type != typeof(Control) &&   // отфильтровываем (исключаем сам Control)
                               !type.IsAbstract &&          // только те, которые можем создать
                               !type.IsNested &&            // не вложенные (они вспомогательные)
                               !type.GetCustomAttributesData().Any(a =>
                               {
                                   // контрол должен быть запрещен к использованию на панели инструментов, чтобы его не мог использовать разработчик
                                   if (a.AttributeType == typeof(ToolboxItemAttribute))
                                   {
                                       if (a.ConstructorArguments.Count == 1)
                                       {
                                           if (a.ConstructorArguments.First().Value is false)
                                           {
                                               return true;
                                           }
                                       }
                                   }
                                   return false;
                               }) &&
                               type.Namespace == "System.Windows.Forms");  // только те, что лежат в указанном пространстве
        }

        private static void WriteStandardControls(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Console.WriteLine(type.FullName);
            }
        }
        static void Main(string[] args)
        {
            // загружаем сборку assembly (где назодятся контроля WinForms)
            var assembly = Assembly.LoadFrom("System.Windows.Forms.dll");

            // получаем контролы и выводим их
            WriteStandardControls(GetStandardControls(assembly));
        }
    }
}
