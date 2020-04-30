#if defined(MATERIAL_HAS_POST_LIGHTING_COLOR)
void blendPostLightingColor(const MaterialInputs material, inout vec4 color) {
#if defined(POST_LIGHTING_BLEND_MODE_OPAQUE)
    color = material.postLightingColor;
#elif defined(POST_LIGHTING_BLEND_MODE_TRANSPARENT)
    color = material.postLightingColor + color * (1.0 - material.postLightingColor.a);
#elif defined(POST_LIGHTING_BLEND_MODE_ADD)
    color += material.postLightingColor;
#elif defined(POST_LIGHTING_BLEND_MODE_MULTIPLY)
    color *= material.postLightingColor;
#elif defined(POST_LIGHTING_BLEND_MODE_SCREEN)
    color += material.postLightingColor * (1.0 - color);
#endif
}
#endif

void main() {
    // See shading_parameters.fs
    // Computes global variables we need to evaluate material and lighting
    computeShadingParams();

    // Initialize the inputs to sensible default values, see material_inputs.fs
    MaterialInputs inputs;
    initMaterial(inputs);

    // Invoke user code
    material(inputs);

    fragColor = evaluateMaterial(inputs);

    bool visualizeCascades = bool(frameUniforms.cascades & 0x10);
    if (visualizeCascades) {
        fragColor.rgb *= uintToColorDebug(getShadowCascade());
    }

#if defined(HAS_FOG)
    vec3 view = getWorldPosition() - getWorldCameraPosition();
    fragColor = fog(fragColor, view);
#endif

#if defined(MATERIAL_HAS_POST_LIGHTING_COLOR)
    blendPostLightingColor(inputs, fragColor);
#endif
}
