#version 120

uniform sampler2D texFramebuffer;

varying vec4 Colour;
varying vec2 Texcoord;

void main()
{
    float mipmapLevel = 3;
//#ifdef GL_ARB_texture_query_lod
//    mipmapLevel = textureQueryLOD(myTexture, textureCoord).x; // NOTE CAPITALIZATION
//#endif
    vec4 texColour = texture2D(texFramebuffer, Texcoord, mipmapLevel);
    gl_FragColor = Colour * texColour;
}