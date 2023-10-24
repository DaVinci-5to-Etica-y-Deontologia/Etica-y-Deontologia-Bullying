using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace Euler
{
    /// <summary>
    /// Clase destinda a agregar Funciones de carpeta y hierarchy a los ScriptableObjects
    /// </summary>
    public abstract class SuperScriptableObject : ScriptableObject
    {

#if UNITY_EDITOR

        public ScriptableObject Parent { get; private set; }


        /// <summary>
        /// Crea un scriptable en el mismo lugar en el que se encuentra el script
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionToCreate">Callback que se ejecuta luego de crear y guardar en disco el scriptable</param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T MakeNew<T>(string name, System.Action<T> actionToCreate) where T : SuperScriptableObject
        {
            if (name == string.Empty)
                name = "Empty";

            var ret = CreateInstance<T>();

            ret.Parent = this;

            var path = AssetDatabase.GetAssetPath(this);

            AssetDatabase.CreateAsset(ret, $"{path.Substring(0, path.LastIndexOf('/'))}/{name}.asset");

            EditorUtility.SetDirty(this);

            EditorUtility.SetDirty(ret);

            EditorUtility.FocusProjectWindow();

            actionToCreate?.Invoke(ret);

            return ret;
        }

        /// <summary>
        /// Crea un scriptable que es hijo del actual ->
        /// ATENCION: no soporta multiples parents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionToCreate">Callback que se ejecuta luego de crear y antes de guardar en disco el scriptable</param>
        /// <returns></returns>
        protected T MakeNewChild<T>(string name, System.Action<T> actionToCreate) where T : SuperScriptableObject
        {
            var ret = CreateInstance<T>();

            ret.Parent = this;

            ret.name = name;

            actionToCreate?.Invoke(ret);

            AssetDatabase.AddObjectToAsset(ret, this);

            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);

            EditorUtility.SetDirty(ret);

            EditorUtility.FocusProjectWindow();

            return ret;
        }

        /// <summary>
        /// destruye el ScriptableObject actual
        /// </summary>
        public virtual void DeleteThis()
        {
            DeleteOther(this);
        }

        /// <summary>
        /// destruye otro Asset
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteOther(Object obj)
        {
            if (obj == null)
                return;

            Undo.DestroyObjectImmediate(obj);

            AssetDatabase.SaveAssets();
        }

#endif
    }

#if UNITY_EDITOR

    public static class Extension
    {
        /// <summary>
        /// Extension que agrega el metodo de deletear todos los elementos de un array que hereden de un SuperScriptableObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        static public void Delete<T>(this T[] array) where T : SuperScriptableObject
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i]?.DeleteThis();
            }
        }

    }

#endif
}


