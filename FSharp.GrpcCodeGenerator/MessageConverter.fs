﻿module MessageConverter

type MessageContext = {
    File: FileContext
    ContainerMessages: Message list
    Message: Message
    OrderedFSFields: FSField list
}

let typeName ctx = Helpers.messageTypeName ctx.Message

let hasExtensionRange (ctx: MessageContext) = ctx.Message.ExtensionRange.Count > 0

let hasExtensions (ctx: MessageContext) = ctx.Message.Extension.Count > 0

let fullClassName (ctx: MessageContext) =
    Helpers.qualifiedInnerNameFromMessages (ctx.Message.Name.Value, ctx.ContainerMessages, ctx.File.File)

let addDeprecatedFlag (ctx: MessageContext) =
    match ctx.Message.Options with
    | ValueSome { Deprecated = ValueSome true } ->
        ctx.File.Writer.Write "[<global.System.ObsoleteAttribute>]"
    | _ -> ()

let hasNestedGeneratedTypes (ctx: MessageContext) =
    if ctx.Message.EnumType.Count > 0 || ctx.Message.OneofDecl.Count > 0
    then true
    elif ctx.Message.NestedType.Count > 0
    then Seq.exists (not << Helpers.isMapEntryMessage) ctx.Message.NestedType
    else false

let writeMessageInterfaces (ctx: MessageContext) =
    ctx.File.Writer.WriteLine "interface global.Google.Protobuf.IBufferMessage with"
    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.InternalMergeFrom(ctx) = me.InternalMergeFrom(&ctx)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.InternalWriteTo(ctx) = me.InternalWriteTo(&ctx)"
    ctx.File.Writer.Outdent()

    let typeName = typeName ctx

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine $"interface global.Google.Protobuf.IExtendableMessage<{typeName}> with"
    else ctx.File.Writer.WriteLine $"interface global.Google.Protobuf.IMessage<{typeName}> with"

    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.Clone() = me.Clone()"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.MergeFrom(other) = me.MergeFrom(other)"
    
    if hasExtensionRange ctx
    then
        ctx.File.Writer.WriteLines [
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.ClearExtension(extension: global.Google.Protobuf.Extension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Clear(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.ClearExtension(extension: global.Google.Protobuf.RepeatedExtension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Clear(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetExtension(extension: global.Google.Protobuf.Extension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Get(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetExtension(extension: global.Google.Protobuf.RepeatedExtension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Get(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetOrInitializeExtension(extension) = global.Google.Protobuf.ExtensionSet.GetOrInitialize(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.HasExtension(extension) = global.Google.Protobuf.ExtensionSet.Has(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.SetExtension(extension, value) = global.Google.Protobuf.ExtensionSet.Set(&me._Extensions, extension, value)"
        ]

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "interface global.Google.Protobuf.IMessage with"
    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.CalculateSize() = me.CalculateSize()"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.MergeFrom(input) = input.ReadRawMessage(me)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.WriteTo(output) = output.WriteRawMessage(me)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member __.Descriptor = {Helpers.reflectionClassName ctx.File.File}.{Helpers.descriptorName (ctx.Message, ctx.ContainerMessages)}()"
    ctx.File.Writer.Outdent()


let writeClrMessageInterfaces (ctx: MessageContext) =
    ctx.File.Writer.WriteLine "interface global.Google.Protobuf.IBufferMessage with"
    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.InternalMergeFrom(ctx) = (me.Backer :> global.Google.Protobuf.IBufferMessage).InternalMergeFrom(&ctx)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.InternalWriteTo(ctx) = (me.Backer :> global.Google.Protobuf.IBufferMessage).InternalWriteTo(&ctx)"
    ctx.File.Writer.Outdent()

    let typeName = typeName ctx

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine $"interface global.Google.Protobuf.IExtendableMessage<{typeName}Clr> with"
    else ctx.File.Writer.WriteLine $"interface global.Google.Protobuf.IMessage<{typeName}Clr> with"

    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member me.Clone() = (me.Backer :> global.Google.Protobuf.IMessage<{typeName}>).Clone() |> {typeName}Clr"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member me.MergeFrom(other) = (me.Backer :> global.Google.Protobuf.IMessage<{typeName}>).MergeFrom(other.Backer)"
    
    if hasExtensionRange ctx
    then
        ctx.File.Writer.WriteLines [
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.ClearExtension(extension: global.Google.Protobuf.Extension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Clear(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.ClearExtension(extension: global.Google.Protobuf.RepeatedExtension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Clear(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetExtension(extension: global.Google.Protobuf.Extension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Get(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetExtension(extension: global.Google.Protobuf.RepeatedExtension<_,'TValue>) = global.Google.Protobuf.ExtensionSet.Get(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.GetOrInitializeExtension(extension) = global.Google.Protobuf.ExtensionSet.GetOrInitialize(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.HasExtension(extension) = global.Google.Protobuf.ExtensionSet.Has(&me._Extensions, extension)"
            "[<global.System.Diagnostics.DebuggerNonUserCodeAttribute>]"
            "member me.SetExtension(extension, value) = global.Google.Protobuf.ExtensionSet.Set(&me._Extensions, extension, value)"
        ]

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "interface global.Google.Protobuf.IMessage with"
    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.CalculateSize() = (me.Backer :> global.Google.Protobuf.IMessage).CalculateSize()"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.MergeFrom(input) = input.ReadRawMessage(me)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "member me.WriteTo(output) = output.WriteRawMessage(me)"
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member __.Descriptor = {Helpers.reflectionClassName ctx.File.File}.{Helpers.descriptorName (ctx.Message, ctx.ContainerMessages)}()"
    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine $"interface global.System.IEquatable<{typeName}Clr> with"
    ctx.File.Writer.Indent()
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member me.Equals(other: {typeName}Clr): bool = me.Backer.Equals(other.Backer)"
    ctx.File.Writer.Outdent()

let writeCloneMethod (ctx: MessageContext) =
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member me.Clone() : {typeName ctx} = {{"
    ctx.File.Writer.Indent()
    
    ctx.File.Writer.WriteLine $"{typeName ctx}._UnknownFields = global.Google.Protobuf.UnknownFieldSet.Clone(me._UnknownFields)"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteCloningCode ctx.File

    if ctx.Message.ExtensionRange.Count > 0
    then ctx.File.Writer.WriteLine $"_Extensions = global.Google.Protobuf.ExtensionSet.Clone(me._Extensions)"

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "}"

let writeMessageSerializationMethods (ctx: MessageContext) =
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member private me.InternalWriteTo(output: byref<global.Google.Protobuf.WriteContext>) ="
    ctx.File.Writer.Indent()

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteSerializationCode ctx.File

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine "if not <| isNull me._Extensions then me._Extensions.WriteTo(&output)"

    ctx.File.Writer.WriteLine "if not <| isNull me._UnknownFields then me._UnknownFields.WriteTo(&output)"

    ctx.File.Writer.Outdent()

    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member private me.CalculateSize() ="
    ctx.File.Writer.Indent()

    ctx.File.Writer.WriteLine $"let mutable size = 0"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteSerializedSizeCode ctx.File

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine "if not <| isNull me._Extensions then size <- size + me._Extensions.CalculateSize()"

    ctx.File.Writer.WriteLine "if not <| isNull me._UnknownFields then size <- size + me._UnknownFields.CalculateSize()"

    ctx.File.Writer.WriteLine "size"
    ctx.File.Writer.Outdent()

let writeMergingMethods (ctx: MessageContext) =
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member private me.MergeFrom(other: {typeName ctx}) ="
    ctx.File.Writer.Indent()

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteMergingCode ctx.File

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine "global.Google.Protobuf.ExtensionSet.MergeFrom(&me._Extensions, other._Extensions)"

    ctx.File.Writer.WriteLine "me._UnknownFields <- global.Google.Protobuf.UnknownFieldSet.MergeFrom(me._UnknownFields, other._UnknownFields)"
    
    ctx.File.Writer.Outdent()

    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"member private me.InternalMergeFrom(input: byref<global.Google.Protobuf.ParseContext>) ="
    ctx.File.Writer.Indent()

    // We can't use seq.Unfold here because ref structs can't be captured by closures

    ctx.File.Writer.WriteLine "let mutable tag = input.ReadTag()"

    ctx.File.Writer.Write "while "
    
    let endTag = Helpers.groupEndTag (ctx.File, ctx.Message, ctx.ContainerMessages)
    if endTag <> 0u
    then ctx.File.Writer.Write $"tag <> {endTag}u && "
    ctx.File.Writer.WriteLine "tag <> 0u do"
    ctx.File.Writer.Indent()

    ctx.File.Writer.WriteLine "match tag with"
    
    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteParsingCode ctx.File
    
    ctx.File.Writer.WriteLine "| _ ->"
    ctx.File.Writer.Indent()
    if hasExtensionRange ctx
    then
        ctx.File.Writer.WriteLines [
            "if not <| global.Google.Protobuf.ExtensionSet.TryMergeFieldFrom(&me._Extensions, &input)"
            "then me._UnknownFields <- global.Google.Protobuf.UnknownFieldSet.MergeFieldFrom(me._UnknownFields, &input)"
        ]
    else ctx.File.Writer.WriteLine "me._UnknownFields <- global.Google.Protobuf.UnknownFieldSet.MergeFieldFrom(me._UnknownFields, &input)"

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "tag <- input.ReadTag()"

    ctx.File.Writer.Outdent()

    ctx.File.Writer.Outdent()

let writeAdditionalOneOfMethods (ctx: MessageContext) =
    let oneOfFields =
        ctx.OrderedFSFields
        |> Seq.choose (function Single _ -> None | OneOf (o, f, true) -> None | OneOf (o, f, false) -> Some (o, f))
    for (oneOf, fields) in oneOfFields do
        let oneOfName = FieldConverter.oneOfPropertyName oneOf
        
        Helpers.writeGeneratedCodeAttribute ctx.File
        ctx.File.Writer.WriteLine $"member me.{oneOfName}Case ="
        ctx.File.Writer.Indent()
        
        ctx.File.Writer.WriteLine $"match me.{oneOfName} with"
        ctx.File.Writer.WriteLine $"| ValueNone -> 0"
        for field in fields do
            ctx.File.Writer.WriteLine $"| ValueSome({FieldConverter.oneOfCaseName (oneOf, field, ctx.Message, ctx.ContainerMessages, ctx.File.File)} _) -> {field.Number.Value}"
        
        ctx.File.Writer.Outdent()

        Helpers.writeGeneratedCodeAttribute ctx.File
        ctx.File.Writer.WriteLine $"member me.Clear{oneOfName}() = me.{oneOfName} <- ValueNone"

        for field in fields do
            Helpers.writeGeneratedCodeAttribute ctx.File
            ctx.File.Writer.WriteLine $"member me.{FieldConverter.propertyName (ctx.Message, field)}"
            ctx.File.Writer.Indent()

            ctx.File.Writer.WriteLine $"with get() ="
            ctx.File.Writer.Indent()
            
            ctx.File.Writer.WriteLine $"match me.{oneOfName} with"

            ctx.File.Writer.WriteLine $"| ValueSome({FieldConverter.oneOfCaseName (oneOf, field, ctx.Message, ctx.ContainerMessages, ctx.File.File)} x) -> x"

            ctx.File.Writer.WriteLine $"| _ -> Unchecked.defaultof<_>"
            
            ctx.File.Writer.Outdent()

            ctx.File.Writer.WriteLine $"and set(x) ="
            ctx.File.Writer.Indent()
            
            ctx.File.Writer.WriteLine $"me.{oneOfName} <- ValueSome({FieldConverter.oneOfCaseName (oneOf, field, ctx.Message, ctx.ContainerMessages, ctx.File.File)} x)"
            
            ctx.File.Writer.Outdent()
            
            ctx.File.Writer.Outdent()

let private requiresPresenceFields (file: File) (field: Field): bool =
    (field.Proto3Optional |> ValueOption.defaultWith (fun _ -> false))
    || (not (field.Type.Value = Google.Protobuf.FSharp.Reflection.FieldDescriptorProto.Types.Type.Message)
        && not (field.Label.Value = Google.Protobuf.FSharp.Reflection.FieldDescriptorProto.Types.Label.Repeated)
        && (field.OneofIndex = ValueNone)
        && (file.Syntax |> ValueOption.defaultWith (fun _ -> "proto2")) = "proto2")

let writeAdditionalOptionalMethods (ctx: MessageContext) =
    let fields =
        ctx.OrderedFSFields
        |> Seq.map (function Single f -> [f] | OneOf (_, f, _) -> f)
        |> Seq.concat

    for f in fields do
        if requiresPresenceFields ctx.File.File f then
            let propertyName = propertyName (ctx.Message, f)
            
            Helpers.writeGeneratedCodeAttribute ctx.File
            ctx.File.Writer.WriteLine $"member me.Has{propertyName} ="
            ctx.File.Writer.Indent()
            
            // If the oneof field is optional there will only be one case and it has been unwrapped
            if f.Proto3Optional |> ValueOption.defaultWith (fun _ -> false) then
                ctx.File.Writer.WriteLine (hasPropertyCheck (ctx.File, ctx.Message, f, "me"))
            else
                ctx.File.Writer.WriteLine $"match me.{propertyName} with"
                ctx.File.Writer.WriteLine $"| ValueNone -> false"
                ctx.File.Writer.WriteLine $"| ValueSome(_) -> true"
            
            ctx.File.Writer.Outdent()

            Helpers.writeGeneratedCodeAttribute ctx.File
            // If the oneof field is optional there will only be one case and it has been unwrapped
            if f.Proto3Optional |> ValueOption.defaultWith (fun _ -> false) then
                ctx.File.Writer.WriteLine $"member me.Clear{propertyName} () = me.{propertyName} <- {Helpers.messageTypeName ctx.Message}.DefaultValue.{propertyName}"
            else
                ctx.File.Writer.WriteLine $"member me.Clear{propertyName} () = me.{propertyName} <- ValueNone"

let rec writeMessageModule (ctx: MessageContext) =
    ctx.File.Writer.WriteLine $"module {Helpers.accessSpecifier ctx.File}{typeName ctx} ="
    ctx.File.Writer.Indent()

    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "let internal DefaultValue = {"
    ctx.File.Writer.Indent()

    ctx.File.Writer.WriteLine $"{typeName ctx}._UnknownFields = null"
    
    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine $"{typeName ctx}._Extensions = null"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteMemberInit ctx.File

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "}"

    // Since we're creating mutable message types, we need to return new instances from the parser factory method below
    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine "let empty () = {"
    ctx.File.Writer.Indent()

    ctx.File.Writer.WriteLine $"{typeName ctx}._UnknownFields = null"
    
    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine $"{typeName ctx}._Extensions = null"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteMemberInit ctx.File

    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "}"

    Helpers.writeGeneratedCodeAttribute ctx.File
    ctx.File.Writer.WriteLine $"let Parser = global.Google.Protobuf.MessageParser<{typeName ctx}>(global.System.Func<_>(empty))"

    for f in ctx.Message.Field do
        ctx.File.Writer.WriteLine $"let {Helpers.fieldConstantName (ctx.Message, f)} = {f.Number.Value}"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteModuleMembers ctx.File

    if hasNestedGeneratedTypes ctx
    then
        ctx.File.Writer.WriteLine "module Types ="
        ctx.File.Writer.Indent()

        for o in ctx.Message.OneofDecl do
            let name = Helpers.snakeToPascalCase false o.Name.Value
            ctx.File.Writer.WriteLine $"type {name} ="
            for f in Helpers.oneOfFields (ctx.Message, o) do
                let conv = SingleFieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
                conv.WriteOneOfCase ctx.File

        for e in ctx.Message.EnumType do
            EnumConverter.writeEnum ctx.File e

        for t in ctx.Message.NestedType do
            if not <| Helpers.isMapEntryMessage t
            then writeMessage (ctx.File, ctx.ContainerMessages @ [ ctx.Message ], t)

        ctx.File.Writer.Outdent()

    if hasExtensions ctx
    then
       ctx.File.Writer.WriteLine "module Extensions ="
       ctx.File.Writer.Indent()

       for e in ctx.Message.Extension do
           let conv = SingleFieldConverterFactory.createWriter (e, ctx.File, Some ctx.Message, ctx.ContainerMessages)
           conv.WriteExtensionCode ctx.File

       ctx.File.Writer.Outdent()
       ctx.File.Writer.WriteLine ""

    ctx.File.Writer.Outdent()

and writeMessage (ctx: FileContext, containerMessages: Message list, msg: Message) =
    let isSyntheticOneOf msg o = 
        (Helpers.oneOfFields (msg, o)).[0].Proto3Optional |> ValueOption.defaultWith (fun _ -> false)
    let ctx = {
        Message = msg
        File = ctx
        ContainerMessages = containerMessages
        OrderedFSFields =
            let nonOneOfFields =
                msg.Field
                |> Seq.filter (fun f -> Helpers.containingOneOf (msg, f) = ValueNone)
                |> Seq.sortBy (fun f -> f.Number)
                |> Seq.map (fun f -> Single f)

            let oneOfFields =
                if isNull msg.OneofDecl
                then Seq.empty
                else
                    msg.OneofDecl
                    |> Seq.map (fun o -> OneOf (o, Helpers.oneOfFields (msg, o), isSyntheticOneOf msg o))

            Seq.toList <| Seq.append nonOneOfFields oneOfFields
    }

    addDeprecatedFlag ctx

    ctx.File.Writer.WriteLine $"type {Helpers.accessSpecifier ctx.File}{typeName ctx} = {{"
    ctx.File.Writer.Indent()

    ctx.File.Writer.WriteLine "mutable _UnknownFields: global.Google.Protobuf.UnknownFieldSet"

    if hasExtensionRange ctx
    then ctx.File.Writer.WriteLine $"mutable _Extensions: global.Google.Protobuf.ExtensionSet<{typeName ctx}>"

    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteMember ctx.File
        
    ctx.File.Writer.Outdent()
    ctx.File.Writer.WriteLine "} with"
    ctx.File.Writer.Indent()

    writeCloneMethod ctx
    writeMessageSerializationMethods ctx
    writeMergingMethods ctx
    writeAdditionalOneOfMethods ctx
    writeAdditionalOptionalMethods ctx

    writeMessageInterfaces ctx

    ctx.File.Writer.Outdent()

    writeMessageModule ctx

    ctx.File.Writer.WriteLine "[<global.Microsoft.FSharp.Core.AllowNullLiteral>]"
    ctx.File.Writer.WriteLine $"type {Helpers.accessSpecifier ctx.File}{typeName ctx}Clr(initialBacker: {typeName ctx}) ="
    ctx.File.Writer.Indent()
    
    ctx.File.Writer.WriteLine $"let mutable backer: {typeName ctx} = initialBacker"
    ctx.File.Writer.WriteLine $"new() = {typeName ctx}Clr({typeName ctx}.DefaultValue)"
    ctx.File.Writer.WriteLine "member val Backer = backer with get"
    ctx.File.Writer.WriteLine $"static member Parser = global.Google.Protobuf.MessageParser<{typeName ctx}Clr>(global.System.Func<_>({typeName ctx}Clr))"
    ctx.File.Writer.WriteLine $"static member Descriptor = {Helpers.reflectionClassName ctx.File.File}.{Helpers.descriptorName (ctx.Message, ctx.ContainerMessages)}()"
    writeClrMessageInterfaces ctx
    for f in ctx.OrderedFSFields do
        let conv = FieldConverterFactory.createWriter (f, ctx.File, Some ctx.Message, ctx.ContainerMessages)
        conv.WriteClrPassthrough ctx.File "backer"

    let oneOfFields =
        ctx.OrderedFSFields
        |> Seq.choose (function Single _ -> None | OneOf (o, f, true) -> None | OneOf (o, f, false) -> Some (o, f))
    for (oneOf, fields) in oneOfFields do
        let oneOfName = FieldConverter.oneOfPropertyName oneOf
        
        Helpers.writeGeneratedCodeAttribute ctx.File
        ctx.File.Writer.WriteLine $"member me.{oneOfName}Case = me.Backer.{oneOfName}Case"
        for field in fields do
            Helpers.writeGeneratedCodeAttribute ctx.File
            ctx.File.Writer.WriteLine $"member me.{FieldConverter.propertyName (ctx.Message, field)}"
            ctx.File.Writer.Indent()

            ctx.File.Writer.WriteLine $"with get() = me.Backer.{FieldConverter.propertyName (ctx.Message, field)}"
            ctx.File.Writer.WriteLine $"and set(v) = me.Backer.{FieldConverter.propertyName (ctx.Message, field)} <- v"
            ctx.File.Writer.Outdent()

    ctx.File.Writer.Outdent()
