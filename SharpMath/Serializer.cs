using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace SharpMath
{
	public static class Serializer
	{
		public static void Serialize(string fileName, object value)
		{
			Serialize(new FileInfo(fileName), value);
		}

		public static void Serialize(FileInfo file, object value)
		{
			using (Stream stream = File.Open(file.FullName, FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, value);
				stream.Close();
			}
		}

		public static void SerializeCompressed(string fileName, object value)
		{
			SerializeCompressed(new FileInfo(fileName), value);
		}

		public static void SerializeCompressed(FileInfo file, object value)
		{
			using (Stream stream = File.Open(file.FullName, FileMode.Create))
			{
				using (GZipStream compressor = new GZipStream(stream, CompressionMode.Compress))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(compressor, value);
					compressor.Close();
				}
				stream.Close();
			}
		}

		public static object Deserialize(string fileName)
		{
			return Deserialize(new FileInfo(fileName));
		}

		public static object Deserialize(FileInfo file)
		{
			object value;
			using (Stream stream = File.Open(file.FullName, FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				value = formatter.Deserialize(stream);
				stream.Close();
			}

			return value;
		}

		public static object DeserializeCompressed(string fileName)
		{
			return DeserializeCompressed(new FileInfo(fileName));
		}

		public static object DeserializeCompressed(FileInfo file)
		{
			object value;
			using (Stream stream = file.OpenRead())
			{
				using (GZipStream decompressor = new GZipStream(stream, CompressionMode.Decompress))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					value = formatter.Deserialize(decompressor);
					decompressor.Close();
				}
				stream.Close();
			}

			return value;
		}

		public static void XmlSerialize<T>(FileInfo file, T value)
		{
			using (Stream stream = File.Open(file.FullName, FileMode.Create))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(stream, value, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName() }));
				stream.Close();
			}
		}

		public static T XmlDeserialize<T>(FileInfo file)
		{
			T value;
			using (Stream stream = File.Open(file.FullName, FileMode.Open))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				value = (T)serializer.Deserialize(stream);
				stream.Close();
			}

			return value;
		}
	}
}
