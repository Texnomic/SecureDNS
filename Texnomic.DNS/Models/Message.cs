﻿using System;
using BinarySerialization;
using Nerdbank.Streams;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Models
{
    public class Message
    {
        //private ushort? Size;

        //[FieldOrder(0)]
        //[FieldBitLength(16)]
        //[FieldEndianness(Endianness.Big)]
        //[JsonIgnore]
        //public ushort Length
        //{
        //    get => Size ?? CalculateLength();
        //    set => Size = value;
        //}

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort ID { get; set; }

        [FieldOrder(6), FieldBitLength(1), JsonIgnore]
        public MessageType MessageType { get; set; }

        [FieldOrder(5), FieldBitLength(4), JsonIgnore]
        public OperationCode OperationCode { get; set; }

        [FieldOrder(4), FieldBitLength(1), JsonIgnore]
        public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        [FieldOrder(3), FieldBitLength(1), JsonPropertyName("TC")]
        public bool Truncated { get; set; }

        [FieldOrder(2), FieldBitLength(1), FieldEndianness(Endianness.Big), JsonPropertyName("RD")]
        public bool RecursionDesired { get; set; }

        [FieldOrder(7)]
        [FieldBitLength(1)]
        [JsonPropertyName("RA")]
        public bool RecursionAvailable { get; set; }

        [FieldOrder(8), FieldBitLength(1), JsonIgnore]
        public int Zero { get; set; } = 0;

        [FieldOrder(9), FieldBitLength(1), JsonPropertyName("AD")]
        public bool AuthenticatedData { get; set; }

        [FieldOrder(10), FieldBitLength(1), JsonPropertyName("CD")]
        public bool CheckingDisabled { get; set; }

        [FieldOrder(11), FieldBitLength(4), FieldEndianness(Endianness.Big), JsonPropertyName("Status")]
        public ResponseCode ResponseCode { get; set; }

        private ushort? _QuestionsCount;

        [FieldOrder(12), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort QuestionsCount
        {
            get => _QuestionsCount ?? (ushort)(Questions?.Count ?? 0);
            set => _QuestionsCount = value;
        }

        private ushort? _AnswersCount;

        [FieldOrder(13), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort AnswersCount
        {
            get => _AnswersCount ?? (ushort)(Answers?.Count ?? 0);
            set => _AnswersCount = value;
        }

        [FieldOrder(14)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort AuthorityCount { get; set; }

        [FieldOrder(15)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort AdditionalCount { get; set; }

        [FieldOrder(16)]
        [FieldCount(nameof(QuestionsCount))]
        //[ItemSubtypeFactory(nameof(Questions), typeof(QuestionFactory))]
        [JsonPropertyName("Question")]
        public List<Question> Questions { get; set; }

        [FieldOrder(17)]
        [FieldCount(nameof(AnswersCount))]
        //[ItemSubtypeFactory(nameof(Answers), typeof(AnswerFactory))]
        [JsonPropertyName("Answer")]
        public List<Answer> Answers { get; set; }

        [FieldOrder(18)]
        [FieldCount(nameof(AuthorityCount))]
        [JsonPropertyName("Authority")]
        public List<Answer> Authority { get; set; }

        [Ignore]
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        private ushort CalculateLength()
        {
            var QuestionsSize = Questions?.Sum(Q => 4 + Q.Domain.Labels.Sum(Label => 2 + Label.Count)) ?? 0;
            var AnswersSize = Answers?.Sum(A => 10 + A.Domain.Labels.Sum(Label => 2 + Label.Count) + A.Length) ?? 0;

            return (ushort)(12 + QuestionsSize + AnswersSize);
        }

        public byte[] ToArray()
        {
            var Serializer = new BinarySerializer();

            //Serializer.MemberSerializing += OnMemberSerializing;
            //Serializer.MemberSerialized += OnMemberSerialized;
            //Serializer.MemberDeserializing += OnMemberDeserializing;
            //Serializer.MemberDeserialized += OnMemberDeserialized;

            return Serializer.Serialize(this);
        }

        private static void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            Console.WriteLine("S-Start: {0} @ {1}", e.MemberName, e.Offset);
        }

        private static void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            var value = e.Value ?? "null";
            Console.WriteLine("S-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
        }

        private static void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            Console.WriteLine("D-Start: {0} @ {1}", e.MemberName, e.Offset);
        }

        private static void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            Console.CursorLeft = e.Context.Depth * 4;
            var value = e.Value ?? "null";
            Console.WriteLine("D-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
        }

        public async Task<byte[]> ToArrayAsync()
        {
            var Serializer = new BinarySerializer();
            return await Serializer.SerializeAsync(this);
        }

        public static Message FromArray(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Message>(Data);
        }
        public static Task<Message> FromArrayAsync(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.DeserializeAsync<Message>(Data);
        }

        public static Message FromArray(ReadOnlySequence<byte> Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Message>(Data.AsStream());
        }


        public string ToJson()
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            return JsonSerializer.Serialize(this, JsonSerializerOptions);
        }

        public async Task<string> ToJsonAsync()
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            await using var Stream = new MemoryStream();
            using var Reader = new StreamReader(Stream);
            await JsonSerializer.SerializeAsync(Stream, this, JsonSerializerOptions);
            return await Reader.ReadToEndAsync();
        }

        public static Message FromJson(string Json)
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            return JsonSerializer.Deserialize<Message>(Json, JsonSerializerOptions);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}