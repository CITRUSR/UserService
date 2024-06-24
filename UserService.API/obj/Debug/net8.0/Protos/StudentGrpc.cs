// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/student.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace UserService {
  public static partial class Student
  {
    static readonly string __ServiceName = "Student.Student";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::UserService.CreateStudentRequest> __Marshaller_Student_CreateStudentRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::UserService.CreateStudentRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::UserService.CreateStudentResponse> __Marshaller_Student_CreateStudentResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::UserService.CreateStudentResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::UserService.DropOutStudentRequest> __Marshaller_Student_DropOutStudentRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::UserService.DropOutStudentRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::UserService.DropOutStudentResponse> __Marshaller_Student_DropOutStudentResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::UserService.DropOutStudentResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::UserService.CreateStudentRequest, global::UserService.CreateStudentResponse> __Method_CreateStudent = new grpc::Method<global::UserService.CreateStudentRequest, global::UserService.CreateStudentResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CreateStudent",
        __Marshaller_Student_CreateStudentRequest,
        __Marshaller_Student_CreateStudentResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::UserService.DropOutStudentRequest, global::UserService.DropOutStudentResponse> __Method_DropOutStudent = new grpc::Method<global::UserService.DropOutStudentRequest, global::UserService.DropOutStudentResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "DropOutStudent",
        __Marshaller_Student_DropOutStudentRequest,
        __Marshaller_Student_DropOutStudentResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::UserService.StudentReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Student</summary>
    [grpc::BindServiceMethod(typeof(Student), "BindService")]
    public abstract partial class StudentBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::UserService.CreateStudentResponse> CreateStudent(global::UserService.CreateStudentRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::UserService.DropOutStudentResponse> DropOutStudent(global::UserService.DropOutStudentRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Student</summary>
    public partial class StudentClient : grpc::ClientBase<StudentClient>
    {
      /// <summary>Creates a new client for Student</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public StudentClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Student that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public StudentClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected StudentClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected StudentClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::UserService.CreateStudentResponse CreateStudent(global::UserService.CreateStudentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return CreateStudent(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::UserService.CreateStudentResponse CreateStudent(global::UserService.CreateStudentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_CreateStudent, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::UserService.CreateStudentResponse> CreateStudentAsync(global::UserService.CreateStudentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return CreateStudentAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::UserService.CreateStudentResponse> CreateStudentAsync(global::UserService.CreateStudentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_CreateStudent, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::UserService.DropOutStudentResponse DropOutStudent(global::UserService.DropOutStudentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return DropOutStudent(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::UserService.DropOutStudentResponse DropOutStudent(global::UserService.DropOutStudentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_DropOutStudent, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::UserService.DropOutStudentResponse> DropOutStudentAsync(global::UserService.DropOutStudentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return DropOutStudentAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::UserService.DropOutStudentResponse> DropOutStudentAsync(global::UserService.DropOutStudentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_DropOutStudent, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override StudentClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new StudentClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(StudentBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_CreateStudent, serviceImpl.CreateStudent)
          .AddMethod(__Method_DropOutStudent, serviceImpl.DropOutStudent).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, StudentBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_CreateStudent, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::UserService.CreateStudentRequest, global::UserService.CreateStudentResponse>(serviceImpl.CreateStudent));
      serviceBinder.AddMethod(__Method_DropOutStudent, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::UserService.DropOutStudentRequest, global::UserService.DropOutStudentResponse>(serviceImpl.DropOutStudent));
    }

  }
}
#endregion
