#version 330 core
#define MAX_LIGHTS 8

struct Light {
    vec3 position;
    vec3 color;
    float intensity;
};

uniform Light lights[MAX_LIGHTS];
uniform int lightCount;
uniform vec3 viewPos;

in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 result = vec3(0.0);

    for (int i = 0; i < lightCount; i++) {
        vec3 lightDir = normalize(lights[i].position - FragPos);
        float diff = max(dot(norm, lightDir), 0.0);

        float distance = length(lights[i].position - FragPos);
        float attenuation = 1.0 / (1.0 + 0.1 * distance + 0.01 * distance * distance);

        vec3 diffuse = lights[i].color * diff * lights[i].intensity * attenuation;
        result += diffuse;
    }

    FragColor = vec4(result, 1.0);
}
