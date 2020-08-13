using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EZWork;
using UnityEngine;

namespace TabToySpace
{
    namespace EZWork
    {
        /// 配合 Tabtoy 使用
        public class EZTable : EZSingleton<EZTable>
        {
            public Table GameTable;

            public void Init()
            {
                var textAssetTableBytes = EZResource.Instance.LoadRes("GameTable") as TextAsset;
                var bytes = textAssetTableBytes.bytes;
                using (MemoryStream stream = new MemoryStream(bytes)) {
                    stream.Position = 0;

                    var reader = new tabtoy.TableReader(stream);
                    GameTable = new Table();

                    try {
                        GameTable.Deserialize(reader);
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        
        }
    }

    
    
}