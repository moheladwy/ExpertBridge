Great! Based on your updates, here's a revised version of your VPS Configuration Manifesto with the new information integrated:

---

# **VPS Configuration Manifesto**

*Last updated: 5-5-2025

## **Purpose**

This document outlines the security, configuration, and maintenance decisions made to protect and stabilize our Virtual Private Server (VPS) and its hosted environments.

---

## **1. Operating System Hardening**

* **Automatic Security Updates**: Enabled unattended upgrades for security patches:
  * `unattended-upgrades` on Debian/Ubuntu
* **Removed Unused Packages**: Removed unnecessary or unused packages to minimize potential vulnerabilities.

---

## **3. Firewall & Network Rules**
* **Fail2Ban**: Installed and configured to ban IPs after repeated failed login attempts.

---

## **4. Malware & Rootkit Scanning**

* **ClamAV**: Installed ClamAV for antivirus protection to scan for potential malware.
* **rkhunter & chkrootkit**: Both rootkit scanners were installed and configured to check the system for known rootkits and trojans.
* **Manual Full Scan**: Ran full scans using ClamAV, rkhunter, and chkrootkit to ensure no existing threats.
* **Cron Jobs for Regular Scans**: A cron job is scheduled to run these tools periodically to ensure ongoing system health and security.


---

## **7. Final Notes**

* **Threat Model Awareness**: This VPS is assumed to be accessible from the public internet and is treated as hostile territory.
* **Zero Trust Mindset**: No internal process or IP is inherently trusted.
* **Minimal Exposure**: Only expose what’s needed—less surface, fewer targets.
