## Typical Job Flow:

#### Job is created by Client (Job Status is Offered)

#### Contractor accepts Job (Job Status is Accepted else Job Status is Declined)

#### Contractor goes to client and is doing the job (Job Status is InProgress)

#### Contractor finished the job (Job Status is PendingClientApproval)

#### Client approves contractor work (Job Status is Completed)

## Valid Moves

### Valid moves which only contractor does:

- Offered -> Accepted
- Accepted -> Inprogress
- Inprogress -> PendingClientApproval
- \<any status that is not completed> -> Cancelled

### Valid moves for client:

- PendingClientApproval -> Completed
- \<any status that is not completed>->Cancelled

## Endpoints Overview

### 1. **Initiate Job Offer**

- **Route:** `POST /api/Jobs/offer`
- **Request Body:** `InitiateJobOfferRequest`
- **Purpose:** Client (author) offers a job to a contractor (worker).
- **Request Example: **
    ```json
    {
      "ContractorProfileId": "worker-profile-id",
      "Title": "Job Title",
      "Description": "Job details...",
      "ProposedRate": 100.0,
      "JobPostingId": "optional-job-posting-id" // OPTIONAL
    }
    ```
- **Response:** `JobResponse` (details of the created job).

---

### 2. **Respond to Job Offer**

- **Route:** `PATCH /api/Jobs/{jobId}/response`
- **Request Body:** `RespondToJobOfferRequest`
- **Purpose:** Contractor accepts or declines a job offer.
- **Request Example:**
    ```json
    {
      "Accept": true // or false
    }
    ```
- **Response:** `JobResponse` (updated job details).

---

### 3. **Update Job Status**

- **Route:** `PATCH /api/Jobs/{jobId}/status`
- **Request Body:** `UpdateJobStatusRequest`
- **Purpose:** Author or worker updates the job status (e.g., to InProgress, Completed, Cancelled).
- **Request Example:**
    ```json
    {
      "NewStatus": "Completed" // or another valid status
    }
    ```
- **Response:** `JobResponse` (updated job details).
- valid states:
  Offered,
  Accepted,
  InProgress,
  Completed,
  PendingClientApproval,
  Declined,
  Cancelled

---

### 4. **Get Job By Id**

- **Route:** `GET /api/Jobs/{jobId}`
- **Purpose:** Get details of a specific job (must be author or worker).
- **Response:** `JobResponse`

---

### 5. **Get Jobs For Current User**

- **Route:** `GET /api/Jobs`
- **Purpose:** Get all jobs where the current user is author or worker.
- **Response:** `List<JobResponse>`

---

## Example `JobResponse` Structure

```json
{
  "Id": "job-id",
  "Title": "Job Title",
  "Description": "Job details...",
  "Status": "Offered",
  "ActualCost": 100.0,
  "StartedAt": null,
  "EndedAt": null,
  "JobPostingId": "job-posting-id",
  "CreatedAt": "2024-05-15T12:00:00Z",
  "UpdatedAt": "2024-05-15T12:00:00Z",
  "AuthorProfile": {
    "ProfileId": "author-profile-id",
    "FirstName": "Client",
    "LastName": "User",
    "ProfilePictureUrl": "..."
  },
  "WorkerProfile": {
    "ProfileId": "worker-profile-id",
    "FirstName": "Contractor",
    "LastName": "User",
    "ProfilePictureUrl": "..."
  }
}
```
