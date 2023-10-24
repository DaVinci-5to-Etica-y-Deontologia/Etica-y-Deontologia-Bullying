using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Euler
{

#if UNITY_EDITOR

    /// <summary>
    /// Clase auxiliar encargada de realizar el transcripto entre el texto y un formato intermedio
    /// </summary>
    public class Parse
    {
        /*Hice una clase y no una funcion para separar los scripts*/

        /// <summary>
        /// Lista de usuarios
        /// </summary>
        public HashSet<string> DataUser { get; private set; }

        /// <summary>
        /// los comentarios estan dentro de una lista, y sus parametros en un array
        /// Se le agrega como primer parametro el ID
        /// </summary>
        public List<PDO<string, string>> DataOrdered { get; private set; }

        public Parse(TextAsset textAsset)
        {
            DataUser = new HashSet<string>();

            DataOrdered = new List<PDO<string, string>>();

            var row = textAsset.text.Trim().Split('\n');

            var aux = row[0].Trim().Split('\t');

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
                auxCol = row[r].Split('\t');

                string[] col = new string[colLength];

                for (int c = 1; c < col.Length; c++)
                {
                    col[c] = auxCol[Mathf.Clamp(c - 1, 0, auxCol.Length - 1)];
                }

                col[0] = r.ToString();

                DataUser.Add(col[1]);

                PDO<string, string> pdo = new PDO<string, string>(keys,col);

                DataOrdered.Add(pdo);
            }
        }
    }


    /// <summary>
    /// Clase basada en la clase de php PDO, donde tenemos un hibrido diccionario/array
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class PDO<K, V> : IEnumerable
    {
        K[] keys;


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

#endif

}


