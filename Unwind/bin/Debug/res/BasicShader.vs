#version 120

attribute vec3 position;
uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

void main()
{
    vec4 newPosition = vec4(position, 1.0f);
    newPosition = modelviewMatrix * newPosition;
    newPosition = projectionMatrix * newPosition;
    gl_Position = newPosition;
}