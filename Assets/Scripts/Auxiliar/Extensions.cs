using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Internal;
using System.Linq;


public static class Extensions
{
    #region Vectors

    /// <summary>
    /// devuelve una magnitud aproximada del vector donde el valor de este es determinado por la mayor de todas sus proyecciones
    /// muy util para el input del movimiento
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    static public float AproxMagnitude(this Vector2 v)
    {
        var n1 = Mathf.Abs(v.x);
        var n2 = Mathf.Abs(v.y);

        return n1 > n2 ? n1 : n2;
    }

    /// <summary>
    /// devuelve una magnitud aproximada del vector donde el valor de este es determinado por la mayor de todas sus proyecciones
    /// muy util para el input del movimiento
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    static public float AproxMagnitude(this Vector3 v)
    {
        var n1 = Mathf.Abs(v.z);

        return (AproxMagnitude(v.Vect3To2()) > n1 ? AproxMagnitude(v.Vect3To2()) : n1);
    }

    /// <summary>
    /// Remplaza el parametro x, por el valor dado
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector3 Vect3_X(ref this Vector3 v, float x)
    {
        v.x = x;

        return v;
    }

    /// <summary>
    /// Remplaza el parametro Y, por el valor dado
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector3 Vect3_Y(ref this Vector3 v, float y)
    {
        v.y = y;

        return v;
    }

    /// <summary>
    /// Remplaza el parametro Z, por el valor dado
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector3 Vect3_Z(ref this Vector3 v, float z)
    {
        v.z = z;

        return v;
    }

    // <summary>
    /// Setea el vector 3 en cero y lo retorna
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>retorna el propio vector</returns>
    static public Vector3 SetZero(ref this Vector3 vector)
    {
        vector.x = 0;
        vector.y = 0;
        vector.z = 0;

        return vector;
    }



    /// <summary>
    /// Crea un vector 2 a partir de un vector 3
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector2 Vect3To2(this Vector3 v)
    {
        return v;
    }


    /// <summary>
    /// Crea un vector 3 a partir de un vector 2
    /// </summary>
    /// <param name="v">Vector que modifica</param>
    /// <returns></returns>
    static public Vector3 Vec2to3(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    /// <summary>
    /// Setea el vector 2 en cero y lo retorna
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>retorna el propio vector</returns>
    static public Vector2 SetZero(ref this Vector2 vector)
    {
        vector.x = 0;
        vector.y = 0;

        return vector;
    }



    #endregion

    #region string

    static public string RichText(this string s, string tag, string valor="")
    {
        if(valor!="")
            return "<"+tag+"="+valor+">"+s+"</"+tag+">" ;
        else
            return "<" + tag + ">" + s + "</" + tag + ">";
    }

    static public string SubstringClamped(this string s, int startIndex)
    {
        startIndex = Mathf.Clamp(startIndex, 0, s.Length);

        return s.Length == 0 ? string.Empty : s.Substring(startIndex);
    }

    static public string SubstringClamped(this string s, int startIndex, int lenght)
    {
        startIndex = Mathf.Clamp(startIndex, 0, s.Length);

        lenght = Mathf.Clamp(lenght, 0, (s.Length) - startIndex);

        return s.Length == 0 ? string.Empty : s.Substring(startIndex, lenght);
    }

    #endregion

    #region Colors
    /// <summary>
    /// Retorna una copia del color con el alpha cambiado
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    static public Color ChangeAlphaCopy(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// Retorna un nuevo color respetando el alpha anterior
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    static public Color ChangeColorCopy(this Color color, Color newColor)
    {
        return new Color(newColor.r, newColor.g, newColor.b, color.a);
    }
    #endregion

    #region unity components
    static public T SetActiveGameObject<T>(this T component, bool active) where T : Component
    {
        component.gameObject.SetActive(active);
        return component;
    }

    static public T SetActive<T>(this T mono, bool active) where T : MonoBehaviour
    {
        mono.enabled = active;
        return mono;
    }

    /// <summary>
    /// Calculo automatico del sqrMagnitude de 2 componentes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <param name="toComp"></param>
    /// <returns></returns>
    static public float SqrDistance<T>(this T component, Component toComp) where T : Component
    {
        if (toComp && component)
            return (toComp.transform.position - component.transform.position).sqrMagnitude;
        else
            return float.PositiveInfinity;
    }

    #endregion

    /// <summary>
    /// Añade o inserta en una lista dependiendo de la posicion deseada
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toAdd"></param>
    /// <param name="insert"></param>
    static public void AddOrInsert<T>(this List<T> list, T toAdd ,int insert)
    {
        if(insert<0 || insert>= list.Count)
        {
            list.Add(toAdd);
        }
        else
        {
            list.Insert(insert, toAdd);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pictionaries"></param>
    /// <param name="levelMultiply"></param>
    /// <returns></returns>
    static public T RandomPic<T>(this Pictionarys<T,int> pictionaries, int levelMultiply = 0)
    {
        float acumTotal = 0;

        float acumPercentage = 0;

        float rng = Random.Range(0, 1f);

        levelMultiply = Mathf.Clamp(levelMultiply, 0, 1000);

        T lastItem = default;

        foreach (var item in pictionaries)
        {
            acumTotal += item.Value;
        }

        float newAcumTotal = 0;

        if (levelMultiply != 0)
            foreach (var item in pictionaries)
            {
                newAcumTotal += item.Value + Mathf.Pow(1.5f, (1 + (1 - (item.Value / acumTotal)) * levelMultiply));
            }
        else
            newAcumTotal = acumTotal;

        foreach (var item in pictionaries)
        {
            acumPercentage += item.Value / newAcumTotal;

            if (levelMultiply != 0)
                acumPercentage += (Mathf.Pow(1.5f, (1 + (1 - (item.Value / acumTotal)) * levelMultiply))) / newAcumTotal;

            if (rng<= acumPercentage)
            {
                return item.Key;
            }

            lastItem = item.Key;
        }

        return lastItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="tupla"></param>
    /// <returns></returns>
    static public IEnumerable<Pictionary<K, V>> ToPictionary<K, V>(this IEnumerable<(K, V)> tupla)
    {
        foreach (var item in tupla)
        {
            yield return new Pictionary<K, V>(item.Item1, item.Item2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="tupla"></param>
    /// <returns></returns>
    static public Pictionarys<K, V> ToPictionarys<K,V>(this IEnumerable<(K,V)> tupla)
    {
        Pictionarys<K, V> pic = new Pictionarys<K, V>();

        foreach (var item in tupla)
        {
            pic.Add(item.Item1,item.Item2);
        }

        return pic;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="pictionary"></param>
    /// <returns></returns>
    static public Pictionarys<K, V> ToPictionarys<K, V>(this IEnumerable<Pictionary<K, V>> pictionary)
    {
        Pictionarys<K, V> pic = new Pictionarys<K, V>();

        foreach (var item in pictionary)
        {
            pic.Add(item);
        }

        return pic;
    }

    /// <summary>
    /// Filtra todos aquellos que no esten en el radio de toComp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    /// <param name="toComp"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    static public IEnumerable<T> InRadiusOf<T>(this IEnumerable<T> comp, Component toComp, float radius) where T : Component
    {
        return comp.Where( (component) => toComp.SqrDistance(component) <= radius*radius);
    }


}
