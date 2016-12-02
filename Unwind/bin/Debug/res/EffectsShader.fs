#version 120

uniform sampler2D texFramebuffer;
uniform float mipmapLevel;

varying vec4 Colour;
varying vec2 Texcoord;

void main()
{
    vec4 texColour = texture2D(texFramebuffer, Texcoord, mipmapLevel);
    gl_FragColor = Colour * texColour;
}