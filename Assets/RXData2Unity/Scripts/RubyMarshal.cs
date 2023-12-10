using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kaitai;

namespace VirtualPhenix.RXData2Unity
{
    public partial class RubyMarshal : KaitaiStruct
    {
        protected byte[] m_version;
        protected Record m_records;
        protected RubyMarshal m_root;
        protected KaitaiStruct m_parent;

        public virtual byte[] Version => m_version;
        public Record Records => m_records;
        public RubyMarshal M_Root => m_root;
        public KaitaiStruct M_Parent => m_parent;

        public static RubyMarshal FromFile(string fileName, bool _parseUnknown = false)
        {
            return new RubyMarshal(new KaitaiStream(fileName), _parseIfUnknown: _parseUnknown);
        }

        public enum Codes
        {
            RubyString = 34,
            ConstNil = 48,
            RubySymbol = 58,
            RubySymbolLink = 59,
            RubyObjectLink = 64,
            ConstFalse = 70,
            InstanceVar = 73,
            RubyStruct = 83,
            ConstTrue = 84,
            RubyArray = 91,
            PackedInt = 105,
            Bignum = 108,
            RubyHash = 123,
            Unknown = 111,
            Unknown_2 = 126,
            Unknown_3 = 127
        }
        public RubyMarshal(KaitaiStream _io, KaitaiStruct _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
        {
            m_parent = _parent;
            m_root = _root ?? this;
            Read(_parseIfUnknown);
        }
        protected virtual void Read(bool _parseIfUnknown = false)
        {
            m_version = m_io.ReadBytes(2);
            if (!((KaitaiStream.ByteArrayCompare(Version, new byte[] { 4, 8 }) == 0)))
            {
                throw new ValidationNotEqualError(new byte[] { 4, 8 }, Version, M_Io, "/seq/0");
            }
            m_records = new Record(m_io, this, m_root, _parseIfUnknown);
        }

        public partial class RubyArray : KaitaiStruct
        {
            protected PackedInt m_numberOfElements;
            protected List<Record> m_elements;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;

            public PackedInt NumElements { get { return m_numberOfElements; } }
            public List<Record> Elements { get { return m_elements; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubyArray FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubyArray(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubyArray(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_numberOfElements = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_elements = new List<Record>();
                for (var i = 0; i < NumElements.Value; i++)
                {
                    m_elements.Add(new Record(m_io, this, m_root, _parseIfUnknown));
                }
            }
        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Bignum">Source</a>
        /// </remarks>
        public partial class Bignum : KaitaiStruct
        {
            protected byte m_sign;
            protected PackedInt m_lengthDiv2;
            protected byte[] m_body;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;

            /// <summary>
            /// A single byte containing `+` for a positive value or `-` for a negative value.
            /// </summary>
            public byte Sign { get { return m_sign; } }

            /// <summary>
            /// Length of bignum body, divided by 2.
            /// </summary>
            public PackedInt LenDiv2 { get { return m_lengthDiv2; } }

            /// <summary>
            /// Bytes that represent the number, see ruby-lang.org docs for reconstruction algorithm.
            /// </summary>
            public byte[] Body { get { return m_body; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static Bignum FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new Bignum(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public Bignum(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_sign = m_io.ReadU1();
                m_lengthDiv2 = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_body = m_io.ReadBytes((LenDiv2.Value * 2));
            }

        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Struct">Source</a>
        /// </remarks>
        public partial class RubyStruct : KaitaiStruct
        {
            protected Record m_name;
            protected PackedInt m_numberOfMembers;
            protected List<Pair> m_members;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;

            /// <summary>
            /// Symbol containing the name of the struct.
            /// </summary>
            public Record Name { get { return m_name; } }

            /// <summary>
            /// Number of members in a struct
            /// </summary>
            public PackedInt NumMembers { get { return m_numberOfMembers; } }
            public List<Pair> Members { get { return m_members; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubyStruct FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubyStruct(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubyStruct(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_name = new Record(m_io, this, m_root, _parseIfUnknown);
                m_numberOfMembers = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_members = new List<Pair>();
                for (var i = 0; i < NumMembers.Value; i++)
                {
                    m_members.Add(new Pair(m_io, this, m_root, _parseIfUnknown));
                }
            }
        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Symbols+and+Byte+Sequence">Source</a>
        /// </remarks>
        public partial class RubySymbol : KaitaiStruct
        {
            protected PackedInt m_length;
            protected string m_name;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;
            public PackedInt Len { get { return m_length; } }
            public string Name { get { return m_name; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubySymbol FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubySymbol(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubySymbol(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_length = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_name = System.Text.Encoding.Default.GetString(m_io.ReadBytes(Len.Value));
                //m_name = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(Len.Value));
            }
        }

        /// <summary>
        /// Ruby uses sophisticated system to pack integers: first `code`
        /// byte either determines packing scheme or carries encoded
        /// immediate value (thus allowing smaller values from -123 to 122
        /// (inclusive) to take only one byte. There are 11 encoding schemes
        /// in total:
        /// 
        /// * 0 is encoded specially (as 0)
        /// * 1..122 are encoded as immediate value with a shift
        /// * 123..255 are encoded with code of 0x01 and 1 extra byte
        /// * 0x100..0xffff are encoded with code of 0x02 and 2 extra bytes
        /// * 0x10000..0xffffff are encoded with code of 0x03 and 3 extra
        ///   bytes
        /// * 0x1000000..0xffffffff are encoded with code of 0x04 and 4
        ///   extra bytes
        /// * -123..-1 are encoded as immediate value with another shift
        /// * -256..-124 are encoded with code of 0xff and 1 extra byte
        /// * -0x10000..-257 are encoded with code of 0xfe and 2 extra bytes
        /// * -0x1000000..0x10001 are encoded with code of 0xfd and 3 extra
        ///    bytes
        /// * -0x40000000..-0x1000001 are encoded with code of 0xfc and 4
        ///    extra bytes
        /// 
        /// Values beyond that are serialized as bignum (even if they
        /// technically might be not Bignum class in Ruby implementation,
        /// i.e. if they fit into 64 bits on a 64-bit platform).
        /// </summary>
        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Fixnum+and+long">Source</a>
        /// </remarks>
        public partial class PackedInt : KaitaiStruct
        {
            protected bool f_isImmediate;
            protected bool _isImmediate;
            public bool IsImmediate
            {
                get
                {
                    if (f_isImmediate)
                        return _isImmediate;
                    _isImmediate = (bool)(((Code > 4) && (Code < 252)));
                    f_isImmediate = true;
                    return _isImmediate;
                }
            }
            protected bool fm_value;
            protected int m_value;
            public int Value
            {
                get
                {
                    if (fm_value)
                        return m_value;
                    m_value = (int)((IsImmediate ? (Code < 128 ? (Code - 5) : (4 - (~(Code) & 127))) : (Code == 0 ? 0 : (Code == 255 ? (Encoded - 256) : (Code == 254 ? (Encoded - 65536) : (Code == 253 ? (((long)(Encoded2 << 16) | Encoded) - 16777216) : (Code == 3 ? ((long)(Encoded2 << 16) | Encoded) : Encoded)))))));
                    fm_value = true;
                    return m_value;
                }
            }
            protected byte m_code;
            protected uint _encoded;
            protected byte _encoded2;
            protected RubyMarshal m_root;
            protected KaitaiStruct m_parent;
            public byte Code { get { return m_code; } }
            public uint Encoded { get { return _encoded; } }

            /// <summary>
            /// One extra byte for 3-byte integers (0x03 and 0xfd), as
            /// there is no standard `u3` type in KS.
            /// </summary>
            public byte Encoded2 { get { return _encoded2; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }

            public static PackedInt FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new PackedInt(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public PackedInt(KaitaiStream _io, KaitaiStruct _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                f_isImmediate = false;
                fm_value = false;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_code = m_io.ReadU1();
                switch (Code)
                {
                    case 4:
                        {
                            _encoded = m_io.ReadU4le();
                            break;
                        }
                    case 1:
                        {
                            _encoded = m_io.ReadU1();
                            break;
                        }
                    case 252:
                        {
                            _encoded = m_io.ReadU4le();
                            break;
                        }
                    case 253:
                        {
                            _encoded = m_io.ReadU2le();
                            break;
                        }
                    case 3:
                        {
                            _encoded = m_io.ReadU2le();
                            break;
                        }
                    case 2:
                        {
                            _encoded = m_io.ReadU2le();
                            break;
                        }
                    case 255:
                        {
                            _encoded = m_io.ReadU1();
                            break;
                        }
                    case 254:
                        {
                            _encoded = m_io.ReadU2le();
                            break;
                        }
                }
                switch (Code)
                {
                    case 3:
                        {
                            _encoded2 = m_io.ReadU1();
                            break;
                        }
                    case 253:
                        {
                            _encoded2 = m_io.ReadU1();
                            break;
                        }
                }
            }

        }
        public partial class Pair : KaitaiStruct
        {
            protected Record m_key;
            protected Record m_value;
            protected RubyMarshal m_root;
            protected KaitaiStruct m_parent;
            public Record Key { get { return m_key; } }
            public Record Value { get { return m_value; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }

            public static Pair FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new Pair(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public Pair(KaitaiStream _io, KaitaiStruct _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_key = new Record(m_io, this, m_root, _parseIfUnknown);
                m_value = new Record(m_io, this, m_root, _parseIfUnknown);
            }
        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Instance+Variables">Source</a>
        /// </remarks>
        public partial class InstanceVar : KaitaiStruct
        {
            protected Record m_object;
            protected PackedInt m_numberOfVariables;
            protected List<Pair> m_variables;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;
            public Record Obj { get { return m_object; } }
            public PackedInt NumVars { get { return m_numberOfVariables; } }
            public List<Pair> Vars { get { return m_variables; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static InstanceVar FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new InstanceVar(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public InstanceVar(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_object = new Record(m_io, this, m_root, _parseIfUnknown);
                m_numberOfVariables = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_variables = new List<Pair>();
                for (var i = 0; i < NumVars.Value; i++)
                {
                    m_variables.Add(new Pair(m_io, this, m_root, _parseIfUnknown));
                }
            }
        }

        /// <summary>
        /// Each record starts with a single byte that determines its type
        /// (`code`) and contents. If necessary, additional info as parsed
        /// as `body`, to be determined by `code`.
        /// </summary>
        public partial class Record : KaitaiStruct
        {
            protected Codes m_code;
            protected KaitaiStruct m_body;
            protected RubyMarshal m_root;
            protected KaitaiStruct m_parent;
            public Codes Code { get { return m_code; } }
            public KaitaiStruct Body { get { return m_body; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }

            public static Record FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new Record(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public Record(KaitaiStream _io, KaitaiStruct _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }

            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_code = ((RubyMarshal.Codes)m_io.ReadU1());
                switch (Code)
                {
                    case RubyMarshal.Codes.PackedInt:
                        {
                            m_body = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.Bignum:
                        {
                            m_body = new Bignum(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubyArray:
                        {
                            m_body = new RubyArray(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubySymbolLink:
                        {
                            m_body = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubyStruct:
                        {
                            m_body = new RubyStruct(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }

                    case RubyMarshal.Codes.RubyString:
                        {
                            m_body = new RubyString(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.InstanceVar:
                        {
                            m_body = new InstanceVar(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubyHash:
                        {
                            m_body = new RubyHash(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubySymbol:
                        {
                            m_body = new RubySymbol(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.RubyObjectLink:
                        {
                            m_body = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                    case RubyMarshal.Codes.Unknown:
                    case RubyMarshal.Codes.Unknown_2:
                        {
                            if (_parseIfUnknown)
                                m_body = new RubyUnknown(m_io, this, m_root, _parseIfUnknown);
                            break;
                        }
                }
            }
        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-Hash+and+Hash+with+Default+Value">Source</a>
        /// </remarks>
        public partial class RubyHash : KaitaiStruct
        {
            protected PackedInt m_numberOfPairs;
            protected List<Pair> m_pairs;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;
            public PackedInt NumPairs { get { return m_numberOfPairs; } }
            public List<Pair> Pairs { get { return m_pairs; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubyHash FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubyHash(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubyHash(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_numberOfPairs = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_pairs = new List<Pair>();
                for (var i = 0; i < NumPairs.Value; i++)
                {
                    m_pairs.Add(new Pair(m_io, this, m_root, _parseIfUnknown));
                }
            }
        }

        /// <remarks>
        /// Reference: <a href="https://docs.ruby-lang.org/en/2.4.0/marshal_rdoc.html#label-String">Source</a>
        /// </remarks>
        public partial class RubyString : KaitaiStruct
        {
            protected PackedInt m_length;
            protected byte[] m_body;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;
            public PackedInt Len { get { return m_length; } }
            public byte[] Body { get { return m_body; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubyString FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubyString(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubyString(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_length = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_body = m_io.ReadBytes(Len.Value);
            }
        }

        public partial class RubyUnknown : KaitaiStruct
        {
            protected PackedInt m_length;
            protected byte[] m_body;
            protected RubyMarshal m_root;
            protected RubyMarshal.Record m_parent;
            public PackedInt Len { get { return m_length; } }
            public byte[] Body { get { return m_body; } }
            public RubyMarshal M_Root { get { return m_root; } }
            public RubyMarshal.Record M_Parent { get { return m_parent; } }

            public static RubyString FromFile(string fileName, bool _parseIfUnknown = false)
            {
                return new RubyString(new KaitaiStream(fileName), _parseIfUnknown: _parseIfUnknown);
            }

            public RubyUnknown(KaitaiStream _io, RubyMarshal.Record _parent = null, RubyMarshal _root = null, bool _parseIfUnknown = false) : base(_io)
            {
                m_parent = _parent;
                m_root = _root;
                Read(_parseIfUnknown);
            }
            protected virtual void Read(bool _parseIfUnknown = false)
            {
                m_length = new PackedInt(m_io, this, m_root, _parseIfUnknown);
                m_body = m_io.ReadBytes(Len.Value);
            }
        }
    }
}