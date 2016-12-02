#version 120

uniform sampler2D texFramebuffer;
uniform float mipmapLevel;

varying vec4 Colour;
varying vec2 Texcoord;

const float blurSize = 1 / 200.0f;

void main()
{
    vec3 colourSum = vec3(0.0f);
    
    for (int x = -4; x <= 4; x++)
    {
        for (int y = -4; y <= 4; y++)
        {
            float dx = Texcoord.x + x * blurSize;
            float dy = Texcoord.y + y * blurSize;
            vec3 pixelColour = texture2D(texFramebuffer, vec2(dx, dy), mipmapLevel).xyz;
            colourSum += pixelColour * pixelColour;
        }
    }
    
    colourSum /= 81;
    colourSum = sqrt(colourSum);
    
    vec3 rgb = Colour.xyz;
    rgb = rgb * Colour.a + colourSum * (1 - Colour.a);
    
    gl_FragColor = vec4(rgb, 1);
}