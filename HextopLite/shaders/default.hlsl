cbuffer FrameData : register(b0)
{
    float time;
    float delta_time;
    float2 resolution;
}

struct VSOutput
{
    float4 Position : SV_Position;
    float2 UV : TEXCOORD0;
};

VSOutput VSMain(uint vertexID : SV_VertexID)
{
    float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
    VSOutput o;
    o.UV = uv;
    o.Position = float4(uv * 2.0 - 1.0, 0.0, 1.0);
    return o;
}

float4 PSMain(VSOutput input) : SV_Target
{
    float2 t = input.UV + time * 0.5;
    t *= 2.0;
    float r = sin(t.x);
    r *= r;
    float g = cos(t.y);
    g *= g;
    return float4(r, g, 0.0, 1.0);
}