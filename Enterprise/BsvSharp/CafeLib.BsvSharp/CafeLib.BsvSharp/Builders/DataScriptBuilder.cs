using System.Collections.Generic;
using System.Linq;
using CafeLib.BsvSharp.Exceptions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.Core.Buffers.Arrays;

namespace CafeLib.BsvSharp.Builders
{
    public class DataScriptBuilder : ScriptBuilder
    {
        private readonly List<ByteArrayBuffer> _dataCache;

        public DataScriptBuilder()
        {
            _dataCache = new List<ByteArrayBuffer>();
        }

        public DataScriptBuilder(byte[] data)
            : this()
        {
            _dataCache.Add(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override ScriptBuilder Add(byte[] data)
        {
            _dataCache.Add(data);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override ScriptBuilder Clear()
        {
            _dataCache.Clear();
            return base.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Script ToScript()
        {
            base.Clear();
            Add(Opcode.OP_FALSE);
            Add(Opcode.OP_RETURN);

            if (_dataCache == null || !_dataCache.Any())
            {
                return base.ToScript();
            }

            _dataCache.ForEach(x =>
            {
                if (x is null || !x.Any()) return;

                var code = (ulong) x.Length switch
                {
                    _ when x.Length < (int) Opcode.OP_PUSHDATA1 => (Opcode) x.Length,
                    _ when x.Length <= 0xff => Opcode.OP_PUSHDATA1,
                    _ when x.Length < 0xffff => Opcode.OP_PUSHDATA2,
                    _ when (ulong) x.Length <= 0xffffffff => Opcode.OP_PUSHDATA4,
                    _ => throw new ScriptException("Data push limit exceeded.")
                };

                Add(code, new VarType(x.ToArray()));
            });

            return base.ToScript();
        }
    }
}
