
4.1.25.0
========
Fixes:
Persistent variables of a FB won't be flagged as persistent by AdsSymbolLoader(V1) (WI 38096)
Fix for accessing MultiDimensionalArrays via FindSymbol (WI 39059)
Fix for ArgumentException in AdsSymbolLoader(V2) FlatMode

4.1.24.0
========
Enhancements:
Support for Alias Types (AliasType, AliasInstance, DynamicAliasInstance)
Support for Union Types (UnionType, UnionInstance, DynamicUnionInstance)
Support for Union Fields (Field, FieldCollection, ReadOnlyFieldCollection)
Support for Oversampling Terminals (OversampleArrayInstance and DynamicOversampleArrayInstance, IOversampleArrayInstance)
Unit Tests for Alias, Union, OversampleArrayInstance

4.1.23.0
========
Fixes: 
Several Errors fixed to support oversampling terminals better in V2 Symbol Browser

Enhancements:
IDynamicSymbol.NormalizedName property for access to the dynamic normalized name. InstanceName and InstancePath now remain like supported from TwinCAT Symbol Browser 
even in dynamic mode.
Implementation of ISymbolLoader, ITypeBinder, ISymbolProvider to enable custom symbol providers. 
Several internal enhancements
Streamlined Unit Tests for TwincAT 3.1.4020

BreakingChanges:
Some Interfaces moved from namespace TwinCAT.TypeSystem to TwinCAT.Ads.TypeSystem to support Custom Symbol Loaders
ISymbol split to ISymbol and IAdsSymbol

New Features:
CommunicationInterceptors to realize deep Extensibility for the TcAdsClient class. First implementations: FailFastHandler, CommunicationStateObserver

4.1.22.0
========
Fix: Access of ArrayElements and element Naming fixed (WI 36239)
Fix: Fixing size of Unicode Strings in TcAdsDataType (WI 36240)
Enhancements: Support of arrays with more than 3 dimensions.
Breaking Changes: NameChange AttributeCollection to TypeAttributeCollection because of Name clashes
Breaking Changes: NameChange ReadOnlyAttributeCollection to ReadOnlyTypeAttributeCollection because of name clashes

4.1.21.0
========
Fix: Correction of InitializePrimitiveType (Type Conversion in ReadSymbol to compatible primitve types fixed)
Fix: Recognition of WSTRING types in AdsSymbolParser corrected

4.1.20.0
========
Fix: TcAdsClient.ReadSymbol don't work with structures (WI: 34853)
Enhancements: Internal code optimizations in TcAdsClient.ReadSymbol

4.1.19.0
========
Fix: Unresolved ArrayElement types can now be browsed again. Enhanced failure tolerance.
Fix: CastException in PointerInstance.OnCreateSubSymbols

4.1.18.0
========
Fix: Inheritance on ContextMask ID on ArrayElement-Instances.

4.1.17.0
========
Endless recursion with Recursive Pointer Data Structures (OutOfMemoryException) fixed.

4.1.16.0
========
Fix Issue Bug 31740: ImageBaseAddress is null for SubSymbols
Fix Issue Bug 31743: Symbolname for elements of arrays with negativ index are invalid	Resolve

4.1.15.0
========
Fix: Corrects breaking changes in classes TimeBase, TIME, TOD, DateBase, DATE, DT
Fix: BOOL built-in datatpye corrected to ADST_BIT (wrong typeid ADST_ULONG8 before)

4.1.14.0
========
Fix: TcAdsSymbolLoader.FindSymbol corrected

4.1.13.0
========
Feature: UINT64 Support for ReadAny
Feature: ITcAdsSymbol5 interface for accessing DataType of Symbol
Fix: Comment forwarding for TcAdsDataType (ArrayInstance fields)

4.1.11.0
========
Feature: Enhanced Tracing

4.1.10.0
=======
Feature: SymbolLoader Settings implementation
Fix: SubItems in InstanceCollections (V2 Symbol Loader)

4.1.9.0
=======
Fix: Support of InstanceName Duplicates (V2 Symbol Loader)
Enhanced Common Error handling