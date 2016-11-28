#version 120

attribute vec4 position;
attribute vec4 colour;
attribute vec2 texcoord;

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

varying vec4 Colour;
varying vec2 Texcoord;

void main()
{
    gl_Position = projectionMatrix * modelviewMatrix * position;
    Colour = colour;
    Texcoord = texcoord;
}