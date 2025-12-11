#lang sicp

(define (read-lines filename)
  (call-with-input-file filename
    (lambda (port)
      (let loop ((lines '()))
        (let ((line (custom-read-line port)))
          (if (eof-object? line)
              (reverse lines)             ; Return all lines in original order
              (loop (cons line lines))))))))

(define (custom-read-line port)
  (let loop ((chars '()))
    (let ((char (read-char port)))
      (cond
        ((eof-object? char)               ; End of file
         (if (null? chars)
             char                         ; Return EOF if no characters were read
             (list->string (reverse chars))))
        ((char=? char #\newline)          ; End of line
         (list->string (reverse chars)))
        (else
         (loop (cons char chars)))))))

(define (split-into-numbers str)
  (let* ((parts (string-split str #\space))
         (num1 (string->number (car parts)))
         (num2 (string->number (cadr parts))))
    (list num1 num2)))

;; Helper function to split a string by a delimiter
(define (string-split str delimiter)
  (let loop ((chars (string->list str))
             (current '())
             (result '())
             (in-delimiter? #f))
    (cond
      ((null? chars)
       (if (null? current)
           (reverse result)
           (reverse (cons (list->string (reverse current)) result))))
      ((char=? (car chars) delimiter)
       (if in-delimiter?
           (loop (cdr chars) current result #t) ; Skip additional delimiters
           (loop (cdr chars) '() (if (null? current)
                                     result
                                     (cons (list->string (reverse current)) result)) #t)))
      (else
       (loop (cdr chars) (cons (car chars) current) result #f)))))


(define (qsort e)
  (if (or (null? e) (<= (length e) 1)) e
      (let loop ((left nil) (right nil)
                   (pivot (car e)) (rest (cdr e)))
            (if (null? rest)
                (append (append (qsort left) (list pivot)) (qsort right))
               (if (<= (car rest) pivot)
                    (loop (append left (list (car rest))) right pivot (cdr rest))
                    (loop left (append right (list (car rest))) pivot (cdr rest)))))))


;;;;;;;;;;;;;;;;;;;;;;;;;; puzzle

(define input (read-lines "AdventOfCode/cache/github/2024_01_input.txt"))

;;(string-split "123   456" #\space)

; (define (parse-input x)
; 	(if (null? x)
; 		'()
; 		(cons (split-into-numbers (car x)) (parse-input (cdr x))
; 	)))
;(define pairs (parse-input input))

(define pairs (map split-into-numbers input))

(define (pick list nth)
	(define (pick-item list nth)
		(if (= nth 0)
			(car list)
			(pick-item (cdr list) (- nth 1))))
	(if (null? list)
		'()
		(cons (pick-item (car list) nth) (pick (cdr list) nth))))

(define list1 (qsort (pick pairs 0)))
(define list2 (qsort (pick pairs 1)))

(define (abs x y)
	(if (< x y) (- y x) (- x y)))

(define (sum-diff s1 s2)
	(if (null? s1)
		0
		(+ (abs (car s1) (car s2)) (sum-diff (cdr s1) (cdr s2)))
	))

(sum-diff list1 list2)


