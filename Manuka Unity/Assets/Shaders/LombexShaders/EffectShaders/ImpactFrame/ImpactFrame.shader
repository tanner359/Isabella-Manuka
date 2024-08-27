// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Lombex/ImpactFrame"
{
	Properties
	{
		_Smoothness("Smoothness", Float) = 1
		_NormalMap("NormalMap", 2D) = "white" {}
		[Toggle(_DISTORTION_ON)] _Distortion("Distortion", Float) = 0
		_Strengh("Strengh", Float) = 1
		_Scale("Scale", Float) = 5
		_Speed("Speed", Range( 0 , 100)) = 0.1
		_DistortionDirection("Distortion Direction", Vector) = (0,1,0,0)
		[Toggle]_ZWrite("ZWrite", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_Culling("Culling", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 8
		[Toggle]_Invert("Invert", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+3000" "IsEmissive" = "true"  }
		Cull [_Culling]
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _DISTORTION_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float _Culling;
		uniform float _ZTest;
		uniform float _ZWrite;
		uniform float _Invert;
		uniform float _Smoothness;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _NormalMap;
		uniform float _Speed;
		uniform float2 _DistortionDirection;
		uniform float _Scale;
		uniform float _Strengh;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 temp_cast_0 = (_Smoothness).xxxx;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float mulTime38 = _Time.y * _Speed;
			float2 normalizeResult40 = normalize( _DistortionDirection );
			float2 uv_TexCoord30 = i.uv_texcoord + ( mulTime38 * normalizeResult40 );
			#ifdef _DISTORTION_ON
				float3 staticSwitch43 = ( UnpackScaleNormal( tex2D( _NormalMap, uv_TexCoord30 ), _Scale ) * _Strengh );
			#else
				float3 staticSwitch43 = float3( 0,0,0 );
			#endif
			float3 DistortionToggle45 = staticSwitch43;
			float4 screenColor2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( DistortionToggle45 , 0.0 ) ).xy);
			float4 Transparency197 = screenColor2;
			float4 smoothstepResult336 = smoothstep( float4( 0,0,0,0 ) , temp_cast_0 , ddx( Transparency197 ));
			float dotResult328 = dot( smoothstepResult336 , smoothstepResult336 );
			float4 temp_cast_3 = (_Smoothness).xxxx;
			float4 smoothstepResult335 = smoothstep( float4( 0,0,0,0 ) , temp_cast_3 , ddy( Transparency197 ));
			float dotResult329 = dot( smoothstepResult335 , smoothstepResult335 );
			float ifLocalVar332 = 0;
			if( dotResult328 <= dotResult329 )
				ifLocalVar332 = dotResult329;
			else
				ifLocalVar332 = dotResult328;
			float temp_output_333_0 = sqrt( ifLocalVar332 );
			float3 temp_cast_4 = ((( _Invert )?( ( 1.0 - temp_output_333_0 ) ):( temp_output_333_0 ))).xxx;
			o.Emission = temp_cast_4;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18713
63;31;1857;1021;2136.638;127.4306;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;49;-2167.971,-245.1909;Inherit;False;1849.393;326.8504;Distortion;12;45;43;34;48;35;31;30;41;40;38;39;37;;0.8962264,0.3170612,0.3170612,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-2117.971,-169.3163;Inherit;False;Property;_Speed;Speed;5;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;39;-2050.971,-100.3164;Inherit;False;Property;_DistortionDirection;Distortion Direction;6;0;Create;True;0;0;0;False;0;False;0,1;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;40;-1820.971,-96.31644;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;38;-1844.971,-163.3163;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1660.969,-118.3164;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-1516.97,-166.3162;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-1440.97,-55.31631;Inherit;False;Property;_Scale;Scale;4;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1112.548,-7.657918;Inherit;False;Property;_Strengh;Strengh;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;48;-1269.246,-195.1909;Inherit;True;Property;_NormalMap;NormalMap;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-934.6138,-26.71627;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;43;-785.7554,-56.68594;Inherit;False;Property;_Distortion;Distortion;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-553.5773,-56.22281;Inherit;False;DistortionToggle;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;198;-2166.661,89.03024;Inherit;False;810.632;299.2022;Transparency;5;197;42;46;1;2;;1,0.9089055,0,0.3137255;0;0
Node;AmplifyShaderEditor.GrabScreenPosition;1;-2141.661,139.688;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;46;-2131.899,305.2325;Inherit;False;45;DistortionToggle;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-1870.22,145.0808;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;2;-1737.745,139.2646;Inherit;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;-1546.029,139.0302;Inherit;False;Transparency;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;327;-2524.491,455.0057;Inherit;False;197;Transparency;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DdyOpNode;330;-2213.986,533.698;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;337;-2248.904,463.4648;Inherit;False;Property;_Smoothness;Smoothness;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DdxOpNode;331;-2209.265,399.2173;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;336;-2013.583,401.8017;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;335;-2019.124,519.0425;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;328;-1834.36,403.4976;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;329;-1833.36,495.4978;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;332;-1677.36,403.4977;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;333;-1491.36,403.4977;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-312.3267,-244.0049;Inherit;False;153.8;272.7;ShaderSettings;3;22;23;21;;0.7906028,0.2310431,0.8301887,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;25;-1316.218,465.0728;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-289.5267,-59.30485;Inherit;False;Property;_Culling;Culling;8;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-290.3267,-194.0049;Inherit;False;Property;_ZTest;ZTest;9;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-290.3267,-127.0049;Inherit;False;Property;_ZWrite;ZWrite;7;1;[Toggle];Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;338;-1142.638,399.5694;Inherit;False;Property;_Invert;Invert;10;0;Create;True;0;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-906.0427,357.9323;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Lombex/ImpactFrame;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;True;22;0;True;21;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;3000;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;1,1,1,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;23;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;39;0
WireConnection;38;0;37;0
WireConnection;41;0;38;0
WireConnection;41;1;40;0
WireConnection;30;1;41;0
WireConnection;48;1;30;0
WireConnection;48;5;31;0
WireConnection;34;0;48;0
WireConnection;34;1;35;0
WireConnection;43;0;34;0
WireConnection;45;0;43;0
WireConnection;42;0;1;0
WireConnection;42;1;46;0
WireConnection;2;0;42;0
WireConnection;197;0;2;0
WireConnection;330;0;327;0
WireConnection;331;0;327;0
WireConnection;336;0;331;0
WireConnection;336;2;337;0
WireConnection;335;0;330;0
WireConnection;335;2;337;0
WireConnection;328;0;336;0
WireConnection;328;1;336;0
WireConnection;329;0;335;0
WireConnection;329;1;335;0
WireConnection;332;0;328;0
WireConnection;332;1;329;0
WireConnection;332;2;328;0
WireConnection;332;3;329;0
WireConnection;332;4;329;0
WireConnection;333;0;332;0
WireConnection;25;0;333;0
WireConnection;338;0;333;0
WireConnection;338;1;25;0
WireConnection;0;2;338;0
ASEEND*/
//CHKSM=DF5F87BC079002520992AA58F8F9B8F23AE45E16