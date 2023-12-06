using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Euler
{
    /// <summary>
    /// Clase auxiliar encargada de realizar el transcripto entre el texto y un formato intermedio
    /// </summary>
    [System.Serializable]
    public class Parse
    {
        /*Hice una clase y no una funcion para separar los scripts*/

        /// <summary>
        /// Levanta cualquier archivo de texto y lo mete dentro de una lista de objetos
        /// Se le agrega como primer parametro el ID, que vendria a ser la posicion original de las filas en el txt
        /// </summary>
        public List<PDO<string, string>> DataOriginalOrder { get; private set; }

        [SerializeField, Tooltip("Recordad trabajar con la codificacion UTF-8")]
        TextAsset textAsset;

        [SerializeField, Tooltip("Separador de las columnas, por defecto en \\t (tabulaciones)")]
        string separator = "\t";

        public Parse Execute()
        {
            DataOriginalOrder = new List<PDO<string, string>>();

            var row = textAsset.text.Trim().Split('\n');

            var aux = row[0].Trim().Split(separator);

            string[] keys = new string[aux.Length + 1];

            keys[0] = "ID";

            string[] auxCol;

            int colLength = keys.Length;

            for (int i = 1; i < keys.Length; i++)
            {
                keys[i] = aux[i - 1].Trim();
            }

            for (int r = 1; r < row.Length; r++)
            {
                auxCol = row[r].Split(separator);

                string[] col = new string[colLength];

                for (int c = 1; c < col.Length; c++)
                {
                    col[c] = auxCol[Mathf.Clamp(c - 1, 0, auxCol.Length - 1)];
                }

                col[0] = r.ToString();

                PDO<string, string> pdo = new PDO<string, string>(keys, col);

                DataOriginalOrder.Add(pdo);
            }

            return this;
        }

        public void Clear()
        {
            DataOriginalOrder.Clear();
            DataOriginalOrder.Capacity = 0;
        }


        public Parse(TextAsset textAsset)
        {
            this.textAsset = textAsset;
            Execute();
        }
    }


    /// <summary>
    /// Clase basada en la clase de php PDO, donde tenemos un hibrido diccionario/array
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [System.Serializable]
    public class PDO<K, V> : IEnumerable
    {
        K[] keys;

        [SerializeField]
        V[] value;

        public int Length => value.Length;

        public PDO(K[] keys, V[] value)
        {
            this.keys = keys;
            this.value = value;
        }

        public V this[int index] => value[index];

        public V this[K str]
        {
            get
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].Equals(str))
                        return value[i];

                }

                throw new System.Exception();
            }
        }

        public override string ToString()
        {
            return string.Join('\t', value);
        }

        public IEnumerator GetEnumerator()
        {
            return value.GetEnumerator();
        }
    }
}


