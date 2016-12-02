#version 120

attribute vec4 position;
attribute vec4 colour;

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

varying vec4 Colour;

void main()
{
    gl_Position = projectionMatrix * modelviewMatrix * position;
    Colour = colour;
}