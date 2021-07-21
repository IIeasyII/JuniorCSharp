using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace JuniorCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AnalysisAssembly analysis = new AnalysisAssembly();

            var info = analysis.AnalysisDLL();

            Console.WriteLine(info);
            Console.ReadKey();
        }
    }

    class AnalysisAssembly
    {
        /// <summary>
        /// Диалоговое окно для выбора папки со сборками
        /// </summary>
        /// <returns>Возвращает пути dll файлов</returns>
        private string[] GetDLLFiles()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var files = Directory.GetFiles(fbd.SelectedPath, "*.dll");

                return files;
            }

            return new string[] { };
        }

        /// <summary>
        /// Анализ Dll сборок
        /// </summary>
        /// <returns>Строка формата:
        /// Type1
        /// - Method1
        /// - Method2
        /// Type2
        /// - Method1
        /// - Method2
        /// </returns>
        public string AnalysisDLL()
        {
            // Получаем все пути dll файлов
            var files = GetDLLFiles();

            Assembly assembly;

            var result = "";

            foreach (var assemblyPath in files)
            {
                // Загружаем сборки
                assembly = Assembly.LoadFrom(assemblyPath);
                // Получаем все типы данной сборки
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // Проводим анализ конкретного типа
                    result += AnalysisType(type);
                }
            }

            return result;
        }

        /// <summary>
        /// Анализ типа на содержание public и protected методов
        /// </summary>
        /// <param name="type">Анализируемый тип</param>
        /// <returns></returns>
        private string AnalysisType(Type type)
        {
            var name = type.Name;
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            var result = name;

            foreach (var method in methods)
            {
                if(method.IsPublic || method.IsFamily)
                {
                    result += "\n - " + method.Name;
                }
            }

            return result + "\n";
        }
    }
}
