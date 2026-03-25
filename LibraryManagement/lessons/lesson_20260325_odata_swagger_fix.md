# Lesson Learned - Swagger vs OData Prefix Conflict

**Date**: 2026-03-25

## What was done
Fixed the "missing EDM model" error in the Library Management API while maintaining Swagger compatibility.

## What went wrong
Initial fix (changing OData prefix to `api`) caused Swagger UI (`/swagger`) to break or fail to discovery APIs correctly. Reverting the prefix to `odata` and then trying to POST to `api/BorrowRequests` caused the EDM model error again.

## Root cause
The error `The request must have an associated EDM model` is specific to `ODataController` helper methods like `Created()`, which expect an OData route for generating metadata. However, forcing the `api/` prefix to be an OData route causes conflicts with Swashbuckle's standard Web API discovery.

## Fix applied
Kept the OData prefix as `odata` and updated all controllers to use standard Web API results instead of OData-specific ones:
- Replaced `Created(result)` with `StatusCode(201, result)` or `CreatedAtAction(...)`.

## How to avoid next time
1.  **Avoid using `ODataController` helper methods** if your API is primarily routed using standard attribute routing (`api/`). Use `CreatedAtAction` or `Ok` instead.
2.  **Keep OData route prefixes distinct** from your main API route prefix to avoid breaking Swagger and other middleware.
3.  **Use `[EnableQuery]` on standard Web API methods** instead of inheriting from `ODataController` if you only need the querying features of OData.
