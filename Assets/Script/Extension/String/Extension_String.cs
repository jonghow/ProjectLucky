using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace DTR_Extension
{
    public static class Extension_String 
    {
        /// <summary>
        ///  Param 2의 스트링 문자열을 Null 이거나, ZeroCount 면 true를 반환합니다.
        /// </summary>
        /// <param name="_checkString">검사할 문자열</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string _checkString)
        {
            return _checkString == null || _checkString.Length == 0;
        }

        public static Vector3 ToVector3(this string _convertString, char[] _deleteChar , char _seperateChar)
        {
            if (_convertString.IsNullOrEmpty()) return Vector3.zero;

            for(int i = 0; i < _deleteChar.Length; i++)
                _convertString = _convertString.Replace(_deleteChar[i].ToString(), "");

            string[] _output = _convertString.ToString().Split(_seperateChar);

            float _x = _output[0].IsNullOrEmpty() == true ? 0f : float.Parse(_output[0]);
            float _y = _output[1].IsNullOrEmpty() == true ? 0f : float.Parse(_output[1]);
            float _z = _output[2].IsNullOrEmpty() == true ? 0f : float.Parse(_output[2]);

            return new Vector3(_x, _y, _z);
        }
    }
}

