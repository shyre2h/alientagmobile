﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace ModIO.Implementation
{
    internal class CompressOperationMultiple : CompressOperationBase
    {
        public IEnumerable<byte[]> data;

        public CompressOperationMultiple(IEnumerable<byte[]> compressed, ProgressHandle progressHandle)
            : base(progressHandle)
        {
            this.data = compressed;
        }

        public override void Cancel()
        {
            cancel = true;
        }

        public override async Task<ResultAnd<MemoryStream>> Compress()
        {
            ResultAnd<MemoryStream> resultAnd = new ResultAnd<MemoryStream>();
            resultAnd.value = new MemoryStream();

            int count = 0;

            using(ZipOutputStream zipStream = new ZipOutputStream(resultAnd.value))
            {
                zipStream.SetLevel(3);

                foreach(var bytes in data)
                {
                    string entryName = $"image_{count}.png";
                    count++;

                    using(MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(bytes, 0, bytes.Length);
                        await CompressStream(entryName, memoryStream, zipStream);
                    }

                    if(cancel || ModIOUnityImplementation.shuttingDown)
                    {
                        return Abort(resultAnd, $"Aborting while zipping images.");
                    }
                }

                zipStream.IsStreamOwner = false;
            }

            resultAnd.result = ResultBuilder.Success;
            return resultAnd;
        }
    }
}
