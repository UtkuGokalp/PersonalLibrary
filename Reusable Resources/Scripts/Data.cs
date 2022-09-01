#nullable enable

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utility.Development
{
    public static class Data
    {
        #region Variables
        private readonly static BinaryFormatter binaryFormatter;
        #endregion

        #region Constructor
        static Data()
        {
            binaryFormatter = new BinaryFormatter();
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the object in binary format. Creates the file and directory if necessary. Returns true if saving was successful. Returns false otherwise.
        /// </summary>
        public static bool Save(string path, object data, bool overwrite = true)
        {
            string? directoryPath = Path.GetDirectoryName(path);
            if (directoryPath == null)
            {
                return false;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            using FileStream stream = File.Open(path, overwrite ? FileMode.Open : FileMode.Append);
            binaryFormatter.Serialize(stream, data);
            return true;
        }
        #endregion

        #region Load<T>
        /// <summary>
        /// Loads the object of the given type, from the given path.
        /// </summary>
        public static T Load<T>(string path)
        {
            string? directoryPath = Path.GetDirectoryName(path);
            if (directoryPath == null || !Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            using FileStream stream = new FileStream(path, FileMode.Open);
            T data = (T)binaryFormatter.Deserialize(stream);

            return data;
        }
        #endregion
    }
}
