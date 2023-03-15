using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    public enum LoadType
    {
        AssetDatabase,
        Bundle,
        //Addressable,
    }
    public class LoaderFactory
    {
        public static AssetLoader Create(LoadType type)
        {
            AssetLoader loader = null;
            switch (type)
            {
                case LoadType.AssetDatabase:
                    loader = new DataBaseLoader();
                    break;
                case LoadType.Bundle:
                    loader = new BundleLoader();
                    break;
                //case LoadType.Addressable:
                    //loader = new AddressablesLoader();
               //     break;
            }
            return loader;
        }
    }
}
