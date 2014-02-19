﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewLife.Serialization
{
    /// <summary>二进制基础类型处理器</summary>
    public class BinaryGeneral : BinaryHandlerBase
    {
        /// <summary>实例化</summary>
        public BinaryGeneral()
        {
            Priority = 10;
        }

        /// <summary>写入一个对象</summary>
        /// <param name="value">目标对象</param>
        /// <param name="type">类型</param>
        /// <returns>是否处理成功</returns>
        public override Boolean Write(Object value, Type type)
        {
            if (value == null && type != typeof(String)) return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    Host.Write((Boolean)value ? 1 : 0);
                    return true;
                case TypeCode.Byte:
                case TypeCode.SByte:
                    Host.Write((Byte)value);
                    return true;
                case TypeCode.Char:
                    Write((Char)value);
                    return true;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    Host.Write((Byte)0);
                    return true;
                case TypeCode.DateTime:
                    Write((DateTime)value);
                    return true;
                case TypeCode.Decimal:
                    Write((Decimal)value);
                    return true;
                case TypeCode.Double:
                    Write((Double)value);
                    return true;
                case TypeCode.Int16:
                    Write((Int16)value);
                    return true;
                case TypeCode.Int32:
                    Write((Int32)value);
                    return true;
                case TypeCode.Int64:
                    Write((Int64)value);
                    return true;
                case TypeCode.Object:
                    break;
                case TypeCode.Single:
                    Write((Single)value);
                    return true;
                case TypeCode.String:
                    Write((String)value);
                    return true;
                case TypeCode.UInt16:
                    Write((UInt16)value);
                    return true;
                case TypeCode.UInt32:
                    Write((UInt32)value);
                    return true;
                case TypeCode.UInt64:
                    Write((UInt64)value);
                    return true;
                default:
                    break;
            }

            if (type == typeof(Guid))
            {
                Write((Guid)value);
                return true;
            }

            return false;
        }

        /// <summary>尝试读取指定类型对象</summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Boolean TryRead(Type type, ref Object value)
        {
            if (type == null)
            {
                if (value == null) return false;
                type = value.GetType();
            }

            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    value = Host.ReadByte() > 0;
                    return true;
                case TypeCode.Byte:
                    value = Host.ReadByte();
                    return true;
                case TypeCode.Char:
                    value = ReadChar();
                    return true;
                case TypeCode.DBNull:
                    value = DBNull.Value;
                    return true;
                case TypeCode.DateTime:
                    value = ReadDateTime();
                    return true;
                case TypeCode.Decimal:
                    value = ReadDecimal();
                    return true;
                case TypeCode.Double:
                    value = ReadDouble();
                    return true;
                case TypeCode.Empty:
                    value = null;
                    return true;
                case TypeCode.Int16:
                    value = ReadInt16();
                    return true;
                case TypeCode.Int32:
                    value = ReadInt32();
                    return true;
                case TypeCode.Int64:
                    value = ReadInt64();
                    return true;
                case TypeCode.Object:
                    break;
                case TypeCode.SByte:
                    value = ReadSByte();
                    return true;
                case TypeCode.Single:
                    value = ReadSingle();
                    return true;
                case TypeCode.String:
                    value = ReadString();
                    return true;
                case TypeCode.UInt16:
                    value = ReadUInt16();
                    return true;
                case TypeCode.UInt32:
                    value = ReadUInt32();
                    return true;
                case TypeCode.UInt64:
                    value = ReadUInt64();
                    return true;
                default:
                    break;
            }

            if (type == typeof(Guid))
            {
                value = new Guid(ReadBytes(16));
                return true;
            }

            return false;
        }

        #region 基元类型写入
        #region 字节
        /// <summary>将一个无符号字节写入</summary>
        /// <param name="value">要写入的无符号字节。</param>
        public virtual void Write(Byte value)
        {
            Host.Write(value);
        }

        /// <summary>将字节数组写入，如果设置了UseSize，则先写入数组长度。</summary>
        /// <param name="buffer">包含要写入的数据的字节数组。</param>
        public virtual void Write(byte[] buffer)
        {
            if (buffer == null)
            {
                Host.WriteSize(0);
                return;
            }

            Host.WriteSize(buffer.Length);
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>将一个有符号字节写入当前流，并将流的位置提升 1 个字节。</summary>
        /// <param name="value">要写入的有符号字节。</param>
        //[CLSCompliant(false)]
        public virtual void Write(sbyte value) { Write((Byte)value); }

        /// <summary>将字节数组部分写入当前流，不写入数组长度。</summary>
        /// <param name="buffer">包含要写入的数据的字节数组。</param>
        /// <param name="offset">buffer 中开始写入的起始点。</param>
        /// <param name="count">要写入的字节数。</param>
        public virtual void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null || buffer.Length < 1 || count <= 0 || offset >= buffer.Length) return;

            Host.Write(buffer, offset, count);
        }

        /// <summary>写入字节数组，自动计算长度</summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="count">数量</param>
        private void Write(Byte[] buffer, Int32 count)
        {
            if (buffer == null) return;

            if (count < 0 || count > buffer.Length) count = buffer.Length;

            Write(buffer, 0, count);
        }
        #endregion

        #region 有符号整数
        /// <summary>将 2 字节有符号整数写入当前流，并将流的位置提升 2 个字节。</summary>
        /// <param name="value">要写入的 2 字节有符号整数。</param>
        public virtual void Write(short value)
        {
            if (Host.EncodeInt)
                WriteEncoded(value);
            else
                WriteIntBytes(BitConverter.GetBytes(value));
        }

        /// <summary>将 4 字节有符号整数写入当前流，并将流的位置提升 4 个字节。</summary>
        /// <param name="value">要写入的 4 字节有符号整数。</param>
        public virtual void Write(int value)
        {
            if (Host.EncodeInt)
                WriteEncoded(value);
            else
                WriteIntBytes(BitConverter.GetBytes(value));
        }

        /// <summary>将 8 字节有符号整数写入当前流，并将流的位置提升 8 个字节。</summary>
        /// <param name="value">要写入的 8 字节有符号整数。</param>
        public virtual void Write(long value)
        {
            if (Host.EncodeInt)
                WriteEncoded(value);
            else
                WriteIntBytes(BitConverter.GetBytes(value));
        }

        /// <summary>判断字节顺序</summary>
        /// <param name="buffer">缓冲区</param>
        void WriteIntBytes(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 1) return;

            // 如果不是小端字节顺序，则倒序
            if (!Host.IsLittleEndian) Array.Reverse(buffer);

            Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region 无符号整数
        /// <summary>将 2 字节无符号整数写入当前流，并将流的位置提升 2 个字节。</summary>
        /// <param name="value">要写入的 2 字节无符号整数。</param>
        //[CLSCompliant(false)]
        public virtual void Write(ushort value) { Write((Int16)value); }

        /// <summary>将 4 字节无符号整数写入当前流，并将流的位置提升 4 个字节。</summary>
        /// <param name="value">要写入的 4 字节无符号整数。</param>
        //[CLSCompliant(false)]
        public virtual void Write(uint value) { Write((Int32)value); }

        /// <summary>将 8 字节无符号整数写入当前流，并将流的位置提升 8 个字节。</summary>
        /// <param name="value">要写入的 8 字节无符号整数。</param>
        //[CLSCompliant(false)]
        public virtual void Write(ulong value) { Write((Int64)value); }
        #endregion

        #region 浮点数
        /// <summary>将 4 字节浮点值写入当前流，并将流的位置提升 4 个字节。</summary>
        /// <param name="value">要写入的 4 字节浮点值。</param>
        public virtual void Write(float value) { Write(BitConverter.GetBytes(value), -1); }

        /// <summary>将 8 字节浮点值写入当前流，并将流的位置提升 8 个字节。</summary>
        /// <param name="value">要写入的 8 字节浮点值。</param>
        public virtual void Write(double value) { Write(BitConverter.GetBytes(value), -1); }
        #endregion

        #region 字符串
        /// <summary>将 Unicode 字符写入当前流，并根据所使用的 Encoding 和向流中写入的特定字符，提升流的当前位置。</summary>
        /// <param name="ch">要写入的非代理项 Unicode 字符。</param>
        public virtual void Write(char ch) { Write(Convert.ToByte(ch)); }

        /// <summary>将字符数组写入当前流，并根据所使用的 Encoding 和向流中写入的特定字符，提升流的当前位置。</summary>
        /// <param name="chars">包含要写入的数据的字符数组。</param>
        public virtual void Write(char[] chars) { Write(chars, 0, chars == null ? 0 : chars.Length); }

        /// <summary>将字符数组部分写入当前流，并根据所使用的 Encoding（可能还根据向流中写入的特定字符），提升流的当前位置。</summary>
        /// <param name="chars">包含要写入的数据的字符数组。</param>
        /// <param name="index">chars 中开始写入的起始点。</param>
        /// <param name="count">要写入的字符数。</param>
        public virtual void Write(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                Host.WriteSize(0);
                return;
            }

            if (chars.Length < 1 || count <= 0 || index >= chars.Length)
            {
                Host.WriteSize(0);
                return;
            }

            // 先用写入字节长度
            var buffer = Host.Encoding.GetBytes(chars, index, count);
            Write(buffer);
        }

        /// <summary>写入字符串</summary>
        /// <param name="value">要写入的值。</param>
        public virtual void Write(String value) { Write(value == null ? null : value.ToCharArray()); }
        #endregion

        #region 时间日期
        /// <summary>将一个时间日期写入</summary>
        /// <param name="value">数值</param>
        public virtual void Write(DateTime value) { Write(value.Ticks); }
        #endregion

        #region 其它
        /// <summary>将一个十进制值写入当前流，并将流位置提升十六个字节。</summary>
        /// <param name="value">要写入的十进制值。</param>
        public virtual void Write(Decimal value)
        {
            Int32[] data = Decimal.GetBits(value);
            for (int i = 0; i < data.Length; i++)
            {
                Write(data[i]);
            }
        }

        /// <summary>写入Guid</summary>
        /// <param name="value">数值</param>
        public virtual void Write(Guid value) { Write(value.ToByteArray(), -1); }
        #endregion
        #endregion

        #region 基元类型读取
        #region 字节
        /// <summary>从当前流中读取下一个字节，并使流的当前位置提升 1 个字节。</summary>
        /// <returns></returns>
        public virtual byte ReadByte() { return Host.ReadByte(); }

        /// <summary>从当前流中将 count 个字节读入字节数组，如果count小于0，则先读取字节数组长度。</summary>
        /// <param name="count">要读取的字节数。</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(int count)
        {
            if (count < 0) count = Host.ReadSize();

            if (count <= 0) return null;

            Byte[] buffer = new Byte[count];
            for (int i = 0; i < count; i++)
            {
                buffer[i] = ReadByte();
            }

            return buffer;
        }

        /// <summary>从此流中读取一个有符号字节，并使流的当前位置提升 1 个字节。</summary>
        /// <returns></returns>
        //[CLSCompliant(false)]
        public virtual sbyte ReadSByte() { return (SByte)ReadByte(); }
        #endregion

        #region 有符号整数
        /// <summary>读取整数的字节数组，某些写入器（如二进制写入器）可能需要改变字节顺序</summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        protected virtual Byte[] ReadIntBytes(Int32 count) { return ReadBytes(count); }

        /// <summary>从当前流中读取 2 字节有符号整数，并使流的当前位置提升 2 个字节。</summary>
        /// <returns></returns>
        public virtual short ReadInt16() { return BitConverter.ToInt16(ReadIntBytes(2), 0); }

        /// <summary>从当前流中读取 4 字节有符号整数，并使流的当前位置提升 4 个字节。</summary>
        /// <returns></returns>
        public virtual int ReadInt32() { return BitConverter.ToInt32(ReadIntBytes(4), 0); }

        /// <summary>从当前流中读取 8 字节有符号整数，并使流的当前位置向前移动 8 个字节。</summary>
        /// <returns></returns>
        public virtual long ReadInt64() { return BitConverter.ToInt64(ReadIntBytes(8), 0); }
        #endregion

        #region 无符号整数
        /// <summary>使用 Little-Endian 编码从当前流中读取 2 字节无符号整数，并将流的位置提升 2 个字节。</summary>
        /// <returns></returns>
        //[CLSCompliant(false)]
        public virtual ushort ReadUInt16() { return (UInt16)ReadInt16(); }

        /// <summary>从当前流中读取 4 字节无符号整数并使流的当前位置提升 4 个字节。</summary>
        /// <returns></returns>
        //[CLSCompliant(false)]
        public virtual uint ReadUInt32() { return (UInt32)ReadInt32(); }

        /// <summary>从当前流中读取 8 字节无符号整数并使流的当前位置提升 8 个字节。</summary>
        /// <returns></returns>
        //[CLSCompliant(false)]
        public virtual ulong ReadUInt64() { return (UInt64)ReadInt64(); }
        #endregion

        #region 浮点数
        /// <summary>从当前流中读取 4 字节浮点值，并使流的当前位置提升 4 个字节。</summary>
        /// <returns></returns>
        public virtual float ReadSingle() { return BitConverter.ToSingle(ReadBytes(4), 0); }

        /// <summary>从当前流中读取 8 字节浮点值，并使流的当前位置提升 8 个字节。</summary>
        /// <returns></returns>
        public virtual double ReadDouble() { return BitConverter.ToDouble(ReadBytes(8), 0); }
        #endregion

        #region 字符串
        /// <summary>从当前流中读取下一个字符，并根据所使用的 Encoding 和从流中读取的特定字符，提升流的当前位置。</summary>
        /// <returns></returns>
        public virtual char ReadChar() { return Convert.ToChar(ReadByte()); }

        /// <summary>从当前流中读取 count 个字符，以字符数组的形式返回数据，并根据所使用的 Encoding 和从流中读取的特定字符，提升当前位置。</summary>
        /// <param name="count">要读取的字符数。</param>
        /// <returns></returns>
        public virtual char[] ReadChars(int count)
        {
            if (count < 0) count = Host.ReadSize();

            //// count个字符可能的最大字节数
            //var max = Settings.Encoding.GetMaxByteCount(count);

            // 首先按最小值读取
            var data = ReadBytes(count);

            return Host.Encoding.GetChars(data);
        }

        /// <summary>从当前流中读取一个字符串。字符串有长度前缀，一次 7 位地被编码为整数。</summary>
        /// <returns></returns>
        public virtual string ReadString()
        {
            // 先读长度
            Int32 n = Host.ReadSize();
            if (n < 0) return null;
            if (n == 0) return String.Empty;

            Byte[] buffer = ReadBytes(n);

            return Host.Encoding.GetString(buffer);
        }
        #endregion

        #region 其它
        /// <summary>从当前流中读取 Boolean 值，并使该流的当前位置提升 1 个字节。</summary>
        /// <returns></returns>
        public virtual bool ReadBoolean() { return ReadByte() != 0; }

        /// <summary>从当前流中读取十进制数值，并将该流的当前位置提升十六个字节。</summary>
        /// <returns></returns>
        public virtual decimal ReadDecimal()
        {
            Int32[] data = new Int32[4];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = ReadInt32();
            }
            return new Decimal(data);
        }

        /// <summary>读取一个时间日期</summary>
        /// <returns></returns>
        public virtual DateTime ReadDateTime() { return new DateTime(ReadInt64()); }
        #endregion
        #endregion

        #region 7位压缩编码整数
        /// <summary>
        /// 以7位压缩格式写入32位整数，小于7位用1个字节，小于14位用2个字节。
        /// 由每次写入的一个字节的第一位标记后面的字节是否还是当前数据，所以每个字节实际可利用存储空间只有后7位。
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>实际写入字节数</returns>
        public Int32 WriteEncoded(Int16 value)
        {
            var list = new List<Byte>();

            Int32 count = 1;
            UInt16 num = (UInt16)value;
            while (num >= 0x80)
            {
                list.Add((byte)(num | 0x80));
                num = (UInt16)(num >> 7);

                count++;
            }
            list.Add((byte)num);

            Write(list.ToArray(), 0, list.Count);

            return count;
        }

        /// <summary>
        /// 以7位压缩格式写入32位整数，小于7位用1个字节，小于14位用2个字节。
        /// 由每次写入的一个字节的第一位标记后面的字节是否还是当前数据，所以每个字节实际可利用存储空间只有后7位。
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>实际写入字节数</returns>
        public Int32 WriteEncoded(Int32 value)
        {
            var list = new List<Byte>();

            Int32 count = 1;
            UInt32 num = (UInt32)value;
            while (num >= 0x80)
            {
                list.Add((byte)(num | 0x80));
                num = num >> 7;

                count++;
            }
            list.Add((byte)num);

            Write(list.ToArray(), 0, list.Count);

            return count;
        }

        /// <summary>
        /// 以7位压缩格式写入64位整数，小于7位用1个字节，小于14位用2个字节。
        /// 由每次写入的一个字节的第一位标记后面的字节是否还是当前数据，所以每个字节实际可利用存储空间只有后7位。
        /// </summary>
        /// <param name="value">数值</param>
        /// <returns>实际写入字节数</returns>
        public Int32 WriteEncoded(Int64 value)
        {
            var list = new List<Byte>();

            Int32 count = 1;
            UInt64 num = (UInt64)value;
            while (num >= 0x80)
            {
                list.Add((byte)(num | 0x80));
                num = num >> 7;

                count++;
            }
            list.Add((byte)num);

            Write(list.ToArray(), 0, list.Count);

            return count;
        }

        /// <summary>获取整数编码后所占字节数</summary>
        /// <param name="value">数值</param>
        /// <returns></returns>
        public static Int32 GetEncodedIntSize(Int64 value)
        {
            Int32 count = 1;
            UInt64 num = (UInt64)value;
            while (num >= 0x80)
            {
                num = num >> 7;

                count++;
            }

            return count;
        }
        #endregion
    }
}