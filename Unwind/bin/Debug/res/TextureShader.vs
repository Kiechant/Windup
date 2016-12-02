#version 120

attribute vec4 position;
attribute vec4 colour;
attribute vec2 texcoord;

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;
uniform float aspect; // Width over height

varying vec4 Colour;
varying vec2 Texcoord;

void main()
{
    gl_Position = projectionMatrix * modelviewMatrix * position;
    Colour = colour;
    
    Texcoord = texcoord;
    if (aspect >= 1)
        Texcoord.x = (1 / aspect) * (Texcoord.x - 0.5f) + 0.5f;
    else
        Texcoord.y = aspect * (Texcoord.y - 0.5f) + 0.5f;
}