# JobPostingsController Overview


## Key Concepts

- **Job Posting:** Created by a client to announce an available job. Contractors can apply to these postings.
- **Job Application:** Submitted by a contractor in response to a job posting.
- **Applicants:** Contractors who have applied to a specific job posting.
- **Job:** Once a contractor is selected/hired, a Job entity is created and managed by the [JobsController](jobs-readme.md).

## Endpoints

### 1. Apply to a Job Posting

- **Route:** `POST /api/JobPostings/{jobPostingId}/apply`
- **Request Body:** `ApplyToJobPostingRequest`
- **Purpose:** Allows a contractor to apply to a job posting.
- **Logic:**
  - Requires authentication.
  - Checks if the user has a profile.
  - Verifies the job posting exists.
  - Prevents duplicate applications by the same contractor.
  - Creates a new `JobApplication` entity and saves it.

---

### 2. Get Applicants for a Job Posting

- **Route:** `GET /api/JobPostings/{jobPostingId}/applicants`
- **Purpose:** Allows the client (author of the posting) to view all applicants for their job posting.
- **Logic:**
  - Requires authentication.
  - Checks if the user is the author of the job posting.
  - Returns a list of applicants, including contractor profile and user details.

---

### 3. Create a Job Posting

- **Route:** `POST /api/JobPostings`
- **Request Body:** `CreateJobPostingRequest`
- **Purpose:** Allows a client to create a new job posting.
- **Logic:**
  - Requires authentication.
  - Checks if the user has a profile.
  - Creates a new `JobPosting` entity and saves it.
  - Returns a `JobPostingResponse` with author profile details.

---

### 4. Get All Job Postings

- **Route:** `GET /api/JobPostings`
- **Purpose:** Returns all job postings in the system.
- **Logic:**
  - Includes author profile and user details for each posting.
  - Returns a list of `JobPostingResponse` objects.

---

## Relationships & Flow

- **JobPostingsController** handles the announcement and application phase.
- **JobsController** (see [jobs-readme.md](jobs-readme.md)) manages the contract after a contractor is selected/hired.
- Typical flow:
  1. Client creates a job posting.
  2. Contractors apply.
  3. Client reviews applicants and selects a contractor (usually via a "Hire" action, which triggers job creation in JobsController).
