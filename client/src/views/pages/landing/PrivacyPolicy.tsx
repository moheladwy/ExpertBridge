import React from "react";

const PrivacyPolicy = () => {
  return (
    <div className="bg-white dark:bg-gray-100 text-gray-900 rounded-2xl shadow-lg max-w-4xl mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">Privacy Policy</h1>

      <p className="mb-4">Effective Date: July 3rd, 2025</p>
      <p className="mb-8">Last Updated: July 3rd, 2025</p>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">1. Introduction</h2>
        <p>
          Welcome to <strong>ExpertBridge</strong> (“we,” “our,” or “us”). Your
          privacy is important to us. This Privacy Policy explains how we
          collect, use, disclose, and safeguard your personal information when
          you use our platform, available via web and mobile (“the Platform”),
          and your rights regarding that information.
        </p>
        <p>
          By using ExpertBridge, you agree to the practices described in this
          Privacy Policy. If you do not agree, please do not use the Platform.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">2. Information We Collect</h2>
        <h3 className="text-xl font-medium mt-4">A. Information You Provide</h3>
        <ul className="list-disc list-inside ml-4">
          <li>Account Information: Name, email, phone number, location, password.</li>
          <li>Profile Data: Biography, profile picture, tags (skills), work history, education, certifications.</li>
          <li>Job Postings and Responses: Job descriptions, location, budget, answers, comments.</li>
          <li>Media Content: Photos, videos, documents uploaded with posts or in chat.</li>
          <li>Reviews & Ratings: Feedback you provide or receive after job completion.</li>
        </ul>
        <h3 className="text-xl font-medium mt-4">B. Automatically Collected Information</h3>
        <ul className="list-disc list-inside ml-4">
          <li>Device Information: IP address, browser type, operating system, device identifiers.</li>
          <li>Usage Data: Time spent, pages visited, features used, interaction logs.</li>
          <li>Cookies & Tracking Technologies: Session identifiers, preferences, authentication tokens.</li>
        </ul>
        <h3 className="text-xl font-medium mt-4">C. Information from Third Parties</h3>
        <p>
          If you sign in or link your account with services like Google or Facebook, we may receive associated information (with your consent).
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">3. How We Use Your Information</h2>
        <ul className="list-disc list-inside ml-4">
          <li>Operate, maintain, and improve the Platform.</li>
          <li>Facilitate job posting, search, communication, and hiring.</li>
          <li>Power AI-based features like content moderation, post recommendations, and tagging.</li>
          <li>Monitor user behavior for abuse prevention, fraud detection, and trust & safety.</li>
          <li>Notify you of messages, opportunities, changes to the platform, or legal updates.</li>
          <li>Personalize your feed and search results.</li>
          <li>Generate aggregated, anonymized analytics for product development.</li>
        </ul>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">4. Sharing Your Information</h2>
        <h3 className="text-xl font-medium mt-4">A. With Other Users</h3>
        <p>Your profile, job posts, comments, ratings, and media may be visible to others as part of the Platform’s functionality.</p>

        <h3 className="text-xl font-medium mt-4">B. With Service Providers</h3>
        <p>Hosting providers, analytics tools, customer support, payment processors, and AI moderation services.</p>

        <h3 className="text-xl font-medium mt-4">C. Legal and Safety Obligations</h3>
        <p>To comply with applicable laws or protect the safety and rights of users or the public.</p>

        <h3 className="text-xl font-medium mt-4">D. Business Transfers</h3>
        <p>If we undergo a merger, acquisition, or sale, your data may be part of the transferred assets.</p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">5. Data Retention</h2>
        <p>
          We retain your personal information as long as necessary for service
          provision, legal compliance, and dispute resolution. You may request
          deletion under Section 8.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">6. Cookies and Tracking</h2>
        <p>
          We use cookies for authentication, preferences, and analytics. You can
          manage cookies in your browser settings.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">7. AI and Automated Processing</h2>
        <p>
          ExpertBridge uses AI to recommend content, tag posts, and moderate
          content. We regularly audit these systems for fairness and accuracy.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">8. Your Rights and Choices</h2>
        <p>
          Depending on your location, you may have rights including access,
          correction, deletion, objection, and portability. To exercise these
          rights, email us at <strong>privacy@expertbridge.com</strong>.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">9. Security</h2>
        <p>
          We use encryption, access control, and monitoring to protect your
          information, though no method is 100% secure.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">10. Children's Privacy</h2>
        <p>
          ExpertBridge is not intended for those under 18. We do not knowingly
          collect data from minors.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">11. International Transfers</h2>
        <p>
          Your data may be processed in jurisdictions outside your own. We
          implement safeguards to protect it.
        </p>
      </section>

      <section className="mb-6">
        <h2 className="text-2xl font-semibold mb-2">12. Changes to This Policy</h2>
        <p>
          We may update this policy. Material changes will be communicated via
          email or in-app notices.
        </p>
      </section>

      <section>
        <h2 className="text-2xl font-semibold mb-2">13. Contact Us</h2>
        <p>
          For any questions or concerns, contact:
          <br />
          <strong>ExpertBridge Team</strong>
          <br />
          Email: <strong>privacy@expertbridge.com</strong>
          <br />
          Address: Cairo, Egypt
        </p>
      </section>
    </div>
  );
};

export default PrivacyPolicy;
