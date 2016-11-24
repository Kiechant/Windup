#version 120

attribute vec4 position;
attribute vec4 colourIn;

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

varying vec4 colour;

void main()
{
    gl_Position = projectionMatrix * modelviewMatrix * position;
    colour = colourIn;
}