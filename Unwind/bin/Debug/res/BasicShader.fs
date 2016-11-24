#version 120

//layout(location = 0) varying vec3 color;
//varying vec2 UV;
//
//uniform sampler2D renderedTexture;

varying vec4 colour;

void main()
{
    gl_FragColor = colour;
//    color = gL_Color;
//    color = texture(renderedTexture, UV).xyz;
}