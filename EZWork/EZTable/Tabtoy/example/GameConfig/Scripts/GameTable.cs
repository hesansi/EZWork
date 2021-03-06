// Generated by github.com/davyxu/tabtoy
// DO NOT EDIT!!
// Version: 3.0.1
using System;
using System.Collections.Generic;

namespace TabToySpace
{ 	
	public enum SexEnum
	{ 
		Male = 0, // 男 
		Female = 1, // 女 
	}
		
	public partial class NPCData : tabtoy.ITableSerializable
	{ 
		public Int32 ID = 0; 
		public string Name = string.Empty; 
		public List<float> Position = new List<float>(); 
		public SexEnum Sex = SexEnum.Male; 

		#region Deserialize Code
		public void Deserialize( tabtoy.TableReader reader )
		{
			UInt32 mamaSaidTagNameShouldBeLong = 0;
            while ( reader.ReadTag(ref mamaSaidTagNameShouldBeLong) )
            {
 				switch (mamaSaidTagNameShouldBeLong)
                { 
                	case 0x20000:
                	{
						reader.ReadInt32( ref ID );
                	}
                	break;
                	case 0x80001:
                	{
						reader.ReadString( ref Name );
                	}
                	break;
                	case 0x6b0002:
                	{
						reader.ReadFloat( ref Position );
                	}
                	break;
                	case 0xa0003:
                	{
						reader.ReadEnum( ref Sex );
                	}
                	break;
                    default:
                    {
                        reader.SkipFiled(mamaSaidTagNameShouldBeLong);                            
                    }
                    break;
				}
			}
		}
		#endregion 
	}
	
	public partial class PlayerData : tabtoy.ITableSerializable
	{ 
		public Int32 ID = 0; 
		public string Name = string.Empty; 
		public Int32 Year = 0; 
		public SexEnum Sex = SexEnum.Male; 
		public List<Int32> Skill = new List<Int32>(); 

		#region Deserialize Code
		public void Deserialize( tabtoy.TableReader reader )
		{
			UInt32 mamaSaidTagNameShouldBeLong = 0;
            while ( reader.ReadTag(ref mamaSaidTagNameShouldBeLong) )
            {
 				switch (mamaSaidTagNameShouldBeLong)
                { 
                	case 0x20000:
                	{
						reader.ReadInt32( ref ID );
                	}
                	break;
                	case 0x80001:
                	{
						reader.ReadString( ref Name );
                	}
                	break;
                	case 0x20002:
                	{
						reader.ReadInt32( ref Year );
                	}
                	break;
                	case 0xa0003:
                	{
						reader.ReadEnum( ref Sex );
                	}
                	break;
                	case 0x660004:
                	{
						reader.ReadInt32( ref Skill );
                	}
                	break;
                    default:
                    {
                        reader.SkipFiled(mamaSaidTagNameShouldBeLong);                            
                    }
                    break;
				}
			}
		}
		#endregion 
	}
	
	public partial class MyKV : tabtoy.ITableSerializable
	{ 
		public string ServerIP = string.Empty; 
		public UInt16 ServerPort = 0; 

		#region Deserialize Code
		public void Deserialize( tabtoy.TableReader reader )
		{
			UInt32 mamaSaidTagNameShouldBeLong = 0;
            while ( reader.ReadTag(ref mamaSaidTagNameShouldBeLong) )
            {
 				switch (mamaSaidTagNameShouldBeLong)
                { 
                	case 0x80000:
                	{
						reader.ReadString( ref ServerIP );
                	}
                	break;
                	case 0x40001:
                	{
						reader.ReadUInt16( ref ServerPort );
                	}
                	break;
                    default:
                    {
                        reader.SkipFiled(mamaSaidTagNameShouldBeLong);                            
                    }
                    break;
				}
			}
		}
		#endregion 
	}
	

	// Combine struct
	public partial class Table
	{ 
		// table: NPCData
		public List<NPCData> NPCData = new List<NPCData>(); 
		// table: PlayerData
		public List<PlayerData> PlayerData = new List<PlayerData>(); 
		// table: MyKV
		public List<MyKV> MyKV = new List<MyKV>(); 

		// Indices 
		public Dictionary<Int32,PlayerData> PlayerDataByID = new Dictionary<Int32,PlayerData>(); 
		public Dictionary<string,PlayerData> PlayerDataByName = new Dictionary<string,PlayerData>(); 

		
		// table: MyKV
		public MyKV GetKeyValue_MyKV()
		{
			return MyKV[0];
		}

		public void ResetData( )
		{   
			NPCData.Clear(); 
			PlayerData.Clear(); 
			MyKV.Clear();  
			PlayerDataByID.Clear(); 
			PlayerDataByName.Clear(); 	
		}
		
		public void Deserialize( tabtoy.TableReader reader )
		{	
			reader.ReadHeader();

			UInt32 mamaSaidTagNameShouldBeLong = 0;
            while ( reader.ReadTag(ref mamaSaidTagNameShouldBeLong) )
            {
				if (mamaSaidTagNameShouldBeLong == 0x6f0000)
				{
                    var tabName = string.Empty;
                    reader.ReadString(ref tabName);
					switch (tabName)
					{ 
						case "NPCData":
						{
							reader.ReadStruct(ref NPCData);	
						}
						break;
						case "PlayerData":
						{
							reader.ReadStruct(ref PlayerData);	
						}
						break;
						case "MyKV":
						{
							reader.ReadStruct(ref MyKV);	
						}
						break;
						default:
						{
							reader.SkipFiled(mamaSaidTagNameShouldBeLong);                            
						}
						break;
					}
				}
			}
				
			foreach( var kv in PlayerData )
			{
				PlayerDataByID[kv.ID] = kv;
			}
				
			foreach( var kv in PlayerData )
			{
				PlayerDataByName[kv.Name] = kv;
			}
			
		}
	}
}
