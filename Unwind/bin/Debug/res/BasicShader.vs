#version 120

attribute vec3 position;

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

void main()
{
    vec4 posOut = vec4(position, 1.0f);
    posOut = projectionMatrix * modelviewMatrix * posOut;
    gl_Position = posOut;
}