import yaml

def GenerateEnumValue(packetName, values):
    enumCode = f"enum {packetName}\n{{\n"
    for value in values:
        enumCode += f"  {value},\n"
    enumCode += "}\n"
    
    return enumCode
        
def GenerateProtocolOverride(packetName):
    method_name = packetName + "Handler"
    return f"""
    public override void {method_name}()
    {{
        // TODO: Add your implementation here.
    }}
    """

def GeneratePacketHandler(packetName):
    method_name = packetName + "Handler"
    return f"""
    public void {method_name}()
    {{
        // TODO: Handle packet
    }}
    """

def GenerateClientPacketHandler(packetName):
    method_name = packetName + "Handler"
    return f"""
    public override void {method_name}()
    {{
        base.{method_name}();
        // TODO: Add client-specific handling here.
    }}
    """

def GeneratePCPacketHandler(packetName):
    method_name = packetName + "Handler"
    return f"""
    public override void {method_name}()
    {{
        // TODO: Implement the method.
    }}
    """

def ProcessPacketGenerate():
    with open(ymlFilePath, 'r') as file:
        ymlData = yaml.load(file, Loader=yaml.SafeLoader)
        
    # Generate PacketType.cs
    enumCode = GenerateEnumValue('PacketType', ymlData['Packet'])
    with open(packetTypeFilePath, 'w') as file:
        file.write(enumCode)
    
    # Generate Protocol.cs
    with open(protocolFilePath, 'w') as file:
        for packetName in ymlData['Packet']:
            file.write(GenerateProtocolOverride(packetName))
    
    # Generate PacketHandler.cs
    with open(packetHandlerFilePath, 'w') as file:
        for packetName in ymlData['Packet']:
            file.write(GeneratePacketHandler(packetName))
            
    # Generate ClientPacketHandler.cs
    with open(clientPacketHandlerFilePath, 'w') as file:
        for packetName in ymlData['Packet']:
            file.write(GenerateClientPacketHandler(packetName))
    
    # Generate PCPacketHandler.cs
    with open(pcPacketHandlerFilePath, 'w') as file:
        for packetName in ymlData['Packet']:
            file.write(GeneratePCPacketHandler(packetName))

# Write file path here
packetTypeFilePath = 'CSharpGameServer/PacketType.cs'
protocolFilePath = 'CSharpGameServer/Protocol.cs'
packetHandlerFilePath = 'CSharpGameServer/PacketHandler.cs'
clientPacketHandlerFilePath = 'CSharpGameServer/Core/ClientManager.cs'
pcPacketHandlerFilePath = 'CSharpGameServer/PC/PCPacketHandler.cs'
ymlFilePath = 'PacketDefine.yml'

ProcessPacketGenerate()
print("Code generated successfully")