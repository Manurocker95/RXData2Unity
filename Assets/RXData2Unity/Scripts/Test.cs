using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Kaitai;
using UnityEngine.Events;

namespace VirtualPhenix.RXData2Unity
{
    public class Test : MonoBehaviour
    {
        [System.Serializable]
        public class KVP
        {
            public List<string> Key;
            public List<string> Value;
        }

        [SerializeField] private bool m_use21 = true;
        [SerializeField] private bool m_parseUnknown = true;
        [SerializeField] private string m_path = "";
        [SerializeField] private List<KVP> m_parsedValues;

        private void Reset()
        {
            m_path = Application.dataPath + "/Game.rxdata";
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_parsedValues = new List<KVP>();

            var path = !m_use21 ? Application.dataPath + "/Game.rxdata" : m_path;
            var data = RubyMarshal.FromFile(path, m_parseUnknown);
            var rec = data.Records;
            var code = rec.Code;

            if (code == RubyMarshal.Codes.RubyHash)
            {
                StartParsing(rec.Body as RubyMarshal.RubyHash);
            }
            else
            {
                Debug.Log("no hash in save");
            }
        }

        public void StartParsing(RubyMarshal.RubyHash _initialHash)
        {
            foreach (var pair in _initialHash.Pairs)
            {
                ParsePair(pair);
            }
        }

        public void ParsePair(RubyMarshal.Pair _pair, bool _isTop = true)
        {
            var k = ParseRecord(true, _pair.Key);
            var v = ParseRecord(false, _pair.Value);

            if (k != null && k.Count > 0 && v != null && v.Count > 0)
                m_parsedValues.Add(new KVP() { Key = k, Value = v });
        }

        public List<string> ParseRecord(bool _isKey, RubyMarshal.Record _record)
        {
            if (_record.Code == 0)
                return null;

            //        Debug.Log("Parsing Record:" + _record.Code);
            switch (_record.Code)
            {
                case RubyMarshal.Codes.Unknown_2:
                case RubyMarshal.Codes.Unknown:
                    return ParseUnknown(_isKey, _record.Body as RubyMarshal.InstanceVar);
                case RubyMarshal.Codes.RubySymbol:
                    return new List<string>() { ParseSymbol(_isKey, _record.Body as RubyMarshal.RubySymbol) };
                case RubyMarshal.Codes.RubyString:
                    return new List<string>() { ParseString(_isKey, _record.Body as RubyMarshal.RubyString) };
                case RubyMarshal.Codes.PackedInt:
                    return new List<string>() { ParseInt(_isKey, _record.Body as RubyMarshal.PackedInt) };
                case RubyMarshal.Codes.Bignum:
                    return new List<string>() { ParseBigInt(_isKey, _record.Body as RubyMarshal.Bignum) };
                case RubyMarshal.Codes.InstanceVar:
                    return ParseInstancedVar(_isKey, _record.Body as RubyMarshal.InstanceVar);
                case RubyMarshal.Codes.RubyArray:
                    return ParseArray(_isKey, _record.Body as RubyMarshal.RubyArray);
                case RubyMarshal.Codes.RubyHash:
                    return ParseHash(_isKey, _record.Body as RubyMarshal.RubyHash);
                case RubyMarshal.Codes.RubySymbolLink:
                    return ParseSymbolLink(_isKey, _record.Body);
                case RubyMarshal.Codes.RubyStruct:
                    return ParseStruct(_isKey, _record.Body as RubyMarshal.RubyStruct);
                case RubyMarshal.Codes.ConstTrue:
                case RubyMarshal.Codes.ConstFalse:
                    return new List<string>() { ParseBoolean(_isKey, _record.Body, true) };
                case RubyMarshal.Codes.Unknown_3:
                    return new List<string>() { ParseSymbol(_isKey, _record.Body as RubyMarshal.RubySymbol) };
            }

            Debug.Log(_record.Code);

            return null;
        }

        public string ParseBoolean(bool _isKey, KaitaiStruct _struct, bool _value)
        {
            Debug.Log("Parsing Boolean - " + _isKey + " - " + _value);
            return _value.ToString();
        }

        public List<string> ParseSymbolLink(bool _isKey, KaitaiStruct _struct)
        {
            Debug.Log("Parsing Symbol Link - " + _isKey + " - ");
            return null;
        }

        public List<string> ParseStruct(bool _isKey, RubyMarshal.RubyStruct _arr)
        {
            Debug.Log("Parsing Struct - " + _isKey + " - " + _arr.Name);

            if (_arr.NumMembers.Value > 0)
            {
                foreach (var v in _arr.Members)
                {
                    ParsePair(v, false);
                }
            }

            return ParseRecord(_isKey, _arr.Name);
        }

        public List<string> ParseHash(bool _isKey, RubyMarshal.RubyHash _arr)
        {
            List<string> elements = new List<string>();
            Debug.Log("Parsing Hash - " + _isKey);
            return elements;
        }
        public List<string> ParseArray(bool _isKey, RubyMarshal.RubyArray _arr)
        {
            List<string> elements = new List<string>();
            Debug.Log("Parsing Array - " + _isKey);

            if (_arr.NumElements.Value > 0)
            {
                foreach (var e in _arr.Elements)
                {
                    var ke = ParseRecord(_isKey, e);

                    if (ke != null && ke.Count > 0)
                    {
                        foreach (var kep in ke)
                        {
                            Debug.Log("Array Element: " + kep);
                        }
                        elements.AddRange(ke);
                    }
                }
            }

            return elements;
        }

        public List<string> ParseUnknown(bool _isKey, RubyMarshal.InstanceVar _unknown, bool _parseVariables = true)
        {
            Debug.Log("Parsing Unknown - " + _isKey);

            if (_unknown == null || _unknown.NumVars == null)
            {
                return new List<string>();
            }

            if (_unknown.NumVars.Value > 0 && _parseVariables)
            {
                foreach (var v in _unknown.Vars)
                {
                    ParsePair(v, false);
                }
            }

            return ParseRecord(_isKey, _unknown.Obj);
        }

        public List<string> ParseInstancedVar(bool _isKey, RubyMarshal.InstanceVar _unknown, bool _parseVariables = true)
        {
            Debug.Log("Parsing InstancedVar - " + _isKey);

            if (_unknown.NumVars.Value > 0 && _parseVariables)
            {
                foreach (var v in _unknown.Vars)
                {
                    ParsePair(v, false);
                }
            }

            return ParseRecord(_isKey, _unknown.Obj);
        }

        public string ParseString(bool _isKey, RubyMarshal.RubyString _string)
        {
            string value = System.Text.Encoding.Default.GetString(_string.Body);
            Debug.Log("Parsing String - " + _isKey + " - " + value);
            return value;
        }

        public string ParseSymbol(bool _isKey, RubyMarshal.RubySymbol _symbol)
        {
            string value = _symbol.Name;
            Debug.Log("Parsing symbol - " + _isKey + " - " + value);
            return value;
        }

        public string ParseInt(bool _isKey, RubyMarshal.PackedInt _int)
        {
            string value = _int.Value.ToString();

            Debug.Log("Parsing symbol - " + _isKey + " - " + value);

            return value;
        }
        public string ParseBigInt(bool _isKey, RubyMarshal.Bignum _int)
        {
            int i = System.BitConverter.ToInt32(_int.Body, 0);
            string value = i.ToString();

            Debug.Log("Parsing symbol - " + _isKey + " - " + value);

            return value;
        }

















        void TT(RubyMarshal.Pair _pair)
        {
            if (_pair.Key.Code == RubyMarshal.Codes.RubySymbol)
            {
                var symbol = _pair.Key.Body as RubyMarshal.RubySymbol;
                string key = symbol.Name;

                if (_pair.Value.Code is RubyMarshal.Codes.Unknown)
                {
                    var value = _pair.Value.Body as RubyMarshal.InstanceVar;
                    if (value.Obj.Code is RubyMarshal.Codes.RubySymbol)
                    {
                        var instancedObj = value.Obj.Body as RubyMarshal.RubySymbol;
                        Debug.Log(key + " - " + instancedObj.Name);

                    }
                    else
                    {
                        Debug.Log(key + " - " + value.Obj.Code);
                    }

                    foreach (var v in value.Vars)
                    {
                        ParsePair(v);
                    }
                }
                else if (_pair.Value.Code is RubyMarshal.Codes.InstanceVar)
                {
                    var value = _pair.Value.Body as RubyMarshal.InstanceVar;

                    if (value.Obj.Code is RubyMarshal.Codes.RubySymbol)
                    {
                        var instancedObj = value.Obj.Body as RubyMarshal.RubySymbol;
                        Debug.Log(key + " 2- " + instancedObj.Name);

                    }
                    else
                    {
                        Debug.Log(key + " 2- " + value.Obj.Code);
                    }
                }
            }
        }


        public void ParsePlayer(RubyMarshal.Record _playerValue)
        {
            Debug.Log("Parsing Player");

            if (_playerValue.Code is RubyMarshal.Codes.Unknown)
            {
                var value = _playerValue.Body as RubyMarshal.InstanceVar;
                Debug.Log("Object Code: " + value.Obj.Code);
                Debug.Log("NumVars: " + value.NumVars.Value);

                foreach (var v in value.Vars)
                {
                    ParsePair(v);
                }
            }
        }


        void ParseSymbols(RubyMarshal.Pair p)
        {
            if (p.Key.Code == RubyMarshal.Codes.RubySymbol)
            {
                var ss = p.Key.Body as RubyMarshal.RubySymbol;
                Debug.Log("Key Name:" + ss.Name);
                if (ss.Name == "@name")
                {
                    if (p.Value.Code == RubyMarshal.Codes.InstanceVar)
                    {
                        var instanced = p.Value.Body as RubyMarshal.InstanceVar;
                        Debug.Log("Value: " + instanced.Obj.Code);
                        if (instanced.Obj.Code == RubyMarshal.Codes.RubyString)
                        {
                            var instancedVal = instanced.Obj.Body as RubyMarshal.RubyString;
                            Debug.Log("Name val: " + System.Text.Encoding.Default.GetString(instancedVal.Body));
                            //instancedVal.OverrideBytes(System.Text.Encoding.Default.GetBytes("ManuUnity"));
                        }

                    }
                }
                else if (ss.Name == "@id")
                {
                    if (p.Value.Code == RubyMarshal.Codes.PackedInt)
                    {
                        var instancedVal = p.Value.Body as RubyMarshal.PackedInt;
                        int rawID = instancedVal.Value;
                        int publicID = rawID & 0xFFFF;
                        int secretID = rawID >> 16;
                        Debug.Log("raw id: " + rawID);
                        Debug.Log("public id: " + publicID);
                        Debug.Log("secret id: " + secretID);
                    }
                }
                else if (ss.Name == "@language")
                {
                    if (p.Value.Code == RubyMarshal.Codes.PackedInt)
                    {
                        var instancedVal = p.Value.Body as RubyMarshal.PackedInt;
                        Debug.Log("lang: " + instancedVal.Value);
                    }
                }
                else if (ss.Name == "@party")
                {
                    if (p.Value.Code == RubyMarshal.Codes.RubyArray)
                    {
                        var instancedVal = p.Value.Body as RubyMarshal.RubyArray;
                        Debug.Log("Elements: " + instancedVal.NumElements.Value);

                        foreach (var pi in instancedVal.Elements)
                        {
                            var pibcode = pi.Code;

                            if (pibcode == RubyMarshal.Codes.RubySymbol)
                            {
                                var pibIns = pi.Body as RubyMarshal.RubySymbol;
                                var speceis = pibIns.Name.Split("@species:");
                            }
                            else
                            {
                                var ssssss = pi.Body as RubyMarshal.RubyUnknown;
                                Debug.Log(pibcode);
                                //Debug.Log(pibcode+ " - " + System.Text.Encoding.Default.GetString(ssssss.Body));
                            }
                        }
                    }
                }
            }


        }
    }
}

